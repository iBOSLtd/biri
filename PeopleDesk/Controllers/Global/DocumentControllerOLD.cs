using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Global;
using System.IO;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentControllerOLD : ControllerBase
    {
        private BlobServiceClient client = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=peopledesk;AccountKey=nsKfOU9FbOydQ5PRN+VajeXaf36Vrq5HF9AsO/93NTehj9/50LBAU12Wh7R5cHeim62PX5SQZ6pG+AStW2uMFg==;EndpointSuffix=core.windows.net");

        private readonly ILogger<DocumentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly PeopleDeskContext _context;

        public DocumentControllerOLD(ILogger<DocumentController> logger,
            IWebHostEnvironment environment,
            PeopleDeskContext _context)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this._context = _context;
        }

        [HttpPost]
        [Route("UploadFile")]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]         // Set the limit to 256 MB
        public async Task<IActionResult> UploadFile(IList<IFormFile> files, long accountId, string tableReferrence, long documentTypeId, long businessUnitId, long createdBy)
        {
            try
            {
                string filename = string.Empty;
                long maxFileSize = 1 * 1024 * 1024; // 1 MB
                List<FileValidationError> errors = new List<FileValidationError>();
                List<FileResponse> responses = new List<FileResponse>();

                foreach (var file in files)
                {
                    if (file.Length > maxFileSize)
                    {
                        long fileSize = file.Length / (1024 * 1024);
                        FileValidationError error = new FileValidationError
                        {
                            FileName = file.Name,
                            Message = "Maximum File Size Allowed (1MB) Your File Size (" + fileSize + "MB)"
                        };
                        errors.Add(error);
                    }
                }

                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }
                else
                {
                    foreach (var file in files)
                    {
                        filename = accountId.ToString() + businessUnitId.ToString() + DateTime.Now.Ticks.ToString() + "_" + (file.FileName == "blob" ? "blob.jpg" : file.FileName);

                        BlobContainerClient container = client.GetBlobContainerClient("saasclientfile");

                        var blobClient = container.GetBlobClient(filename);

                        await blobClient.UploadAsync(file.OpenReadStream());

                        if (!string.IsNullOrEmpty(blobClient.Name))
                        {
                            GlobalFileUrl obj = new GlobalFileUrl
                            {
                                IntAccountId = accountId,
                                IntBusinessUnitId = businessUnitId,
                                StrTableReferrence = tableReferrence,
                                StrRefferenceDescription = "Dcoument file for " + tableReferrence,
                                IntDocumentTypeId = documentTypeId,
                                StrFileServerId = blobClient.Name,
                                StrDocumentName = file.FileName,
                                NumFileSize = Convert.ToDecimal(file.Length),
                                StrFileExtension = Path.GetExtension(file.FileName),
                                StrServerLocation = "Azure File Server",
                                IntCreatedBy = createdBy,
                                DteCreatedAt = DateTime.Now,
                            };
                            await _context.GlobalFileUrls.AddAsync(obj);
                            await _context.SaveChangesAsync();

                            FileResponse response = new FileResponse
                            {
                                globalFileUrlId = obj.IntDocumentId,
                                fileName = file.FileName
                            };
                            responses.Add(response);
                        };
                    }
                    return Ok(responses);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(long id)
        {
            try
            {
                GlobalFileUrl globalFileUrl = await _context.GlobalFileUrls.AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.IntDocumentId == id);

                if (globalFileUrl != null)
                {
                    BlobContainerClient container = client.GetBlobContainerClient("saasclientfile");

                    var blobClient = container.GetBlobClient(globalFileUrl.StrFileServerId);
                    var downloadContent = await blobClient.DownloadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await downloadContent.Value.Content.CopyToAsync(ms);

                        return new FileContentResult(ms.ToArray(), "application/" + globalFileUrl.StrFileExtension.Remove(0, 1))
                        {
                            FileDownloadName = Guid.NewGuid().ToString() + globalFileUrl.StrFileExtension,
                        };
                    }
                }
                else
                {
                    throw new Exception("Data not found");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        [HttpGet]
        [Route("DeleteUploadedFile")]
        public async Task<IActionResult> DeleteUploadedFile(long id)
        {
            try
            {
                GlobalFileUrl globalFileUrl = await _context.GlobalFileUrls.FirstOrDefaultAsync(x => x.IntDocumentId == id);
                globalFileUrl.IsActive = false;

                _context.GlobalFileUrls.Update(globalFileUrl);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully"
                };
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        [Route("HardDeleteUploadedFile")]
        [HttpGet]
        public async Task<IActionResult> HardDeleteUploadedFile(long id)
        {
            GlobalFileUrl globalFileUrl = await _context.GlobalFileUrls.FirstOrDefaultAsync(x => x.IntDocumentId == id);
            globalFileUrl.IsActive = false;

            _context.GlobalFileUrls.Update(globalFileUrl);
            await _context.SaveChangesAsync();

            BlobContainerClient container = client.GetBlobContainerClient("saasclientfile");

            var blobClient = container.GetBlobClient(globalFileUrl.StrFileServerId);
            await blobClient.DeleteAsync();

            MessageHelper res = new MessageHelper
            {
                StatusCode = 200,
                Message = "Deleted Successfully"
            };

            return Ok();
        }

        [HttpPost]
        [Route("UploadFileBaseSixtyFour")]
        public async Task<IActionResult> UploadFileBaseSixtyFour([FromBody] List<BaseSixtyFourDTO> files, long accountId, string tableReferrence, long documentTypeId, long businessUnitId, long createdBy)
        {
            List<FileResponse> responses = new List<FileResponse>();

            foreach (var a in files)
            {
                if (a.Data.Length > 0)
                {
                    byte[] bytes = Convert.FromBase64String(a.Data);

                    Stream stream = null;
                    stream = new MemoryStream(bytes);

                    string filename = accountId.ToString() + businessUnitId.ToString() + DateTime.Now.Ticks.ToString() + "_" + "app_blob.jpg";

                    var file = File(stream, "image/jpg", filename);

                    BlobContainerClient container = client.GetBlobContainerClient("saasclientfile");

                    var blobClient = container.GetBlobClient(filename);

                    await blobClient.UploadAsync(file.FileStream);

                    if (!string.IsNullOrEmpty(blobClient.Name))
                    {
                        GlobalFileUrl obj = new GlobalFileUrl
                        {
                            IntAccountId = accountId,
                            IntBusinessUnitId = businessUnitId,
                            StrTableReferrence = tableReferrence,
                            StrRefferenceDescription = "Dcoument file for " + tableReferrence,
                            IntDocumentTypeId = documentTypeId,
                            StrFileServerId = blobClient.Name,
                            StrDocumentName = filename,
                            NumFileSize = Convert.ToDecimal(stream.Length),
                            StrFileExtension = ".jpg",
                            StrServerLocation = "Azure File Server",
                            IntCreatedBy = createdBy,
                            DteCreatedAt = DateTime.Now,
                        };
                        await _context.GlobalFileUrls.AddAsync(obj);
                        await _context.SaveChangesAsync();

                        FileResponse response = new FileResponse
                        {
                            globalFileUrlId = obj.IntDocumentId,
                            fileName = filename
                        };
                        responses.Add(response);
                    };
                }
            }
            return Ok(responses);
        }

        #region Upload Profile Picture

        [HttpPost]
        [Route("UploadProfilePicture")]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]         // Set the limit to 256 MB
        public async Task<IActionResult> UploadProfilePicture(IList<IFormFile> files, long accountId, string tableReferrence, long documentTypeId, long businessUnitId, long createdBy, long employeeId)
        {
            try
            {
                if (employeeId > 0)
                {
                    string filename = string.Empty;
                    long maxFileSize = 1 * 1024 * 1024; // 1 MB
                    List<FileValidationError> errors = new List<FileValidationError>();
                    List<FileResponse> responses = new List<FileResponse>();

                    foreach (var file in files)
                    {
                        if (file.Length > maxFileSize)
                        {
                            long fileSize = file.Length / (1024 * 1024);
                            FileValidationError error = new FileValidationError
                            {
                                FileName = file.Name,
                                Message = "Maximum File Size Allowed (1MB) Your File Size (" + fileSize + "MB)"
                            };
                            errors.Add(error);
                        }
                    }

                    if (errors.Count > 0)
                    {
                        return NotFound(errors);
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            filename = accountId.ToString() + businessUnitId.ToString() + DateTime.Now.Ticks.ToString() + "_" + (file.FileName == "blob" ? "blob.jpg" : file.FileName);

                            BlobContainerClient container = client.GetBlobContainerClient("saasclientfile");

                            var blobClient = container.GetBlobClient(filename);

                            await blobClient.UploadAsync(file.OpenReadStream());

                            if (!string.IsNullOrEmpty(blobClient.Name))
                            {
                                GlobalFileUrl obj = new GlobalFileUrl
                                {
                                    IntAccountId = accountId,
                                    IntBusinessUnitId = businessUnitId,
                                    StrTableReferrence = tableReferrence,
                                    StrRefferenceDescription = "Dcoument file for " + tableReferrence,
                                    IntDocumentTypeId = documentTypeId,
                                    StrFileServerId = blobClient.Name,
                                    StrDocumentName = file.FileName,
                                    NumFileSize = Convert.ToDecimal(file.Length),
                                    StrFileExtension = Path.GetExtension(file.FileName),
                                    StrServerLocation = "Azure File Server",
                                    IntCreatedBy = createdBy,
                                    DteCreatedAt = DateTime.Now,
                                };
                                _context.GlobalFileUrls.Add(obj);
                                _context.SaveChanges();

                                EmpEmployeePhotoIdentity PhotoIdentity = _context.EmpEmployeePhotoIdentities.AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId).AsNoTracking().FirstOrDefault();
                                if (PhotoIdentity != null)
                                {
                                    PhotoIdentity.IntProfilePicFileUrlId = obj.IntDocumentId;

                                    _context.EmpEmployeePhotoIdentities.Update(PhotoIdentity);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    EmpEmployeePhotoIdentity newObj = new EmpEmployeePhotoIdentity
                                    {
                                        IntEmployeeBasicInfoId = employeeId,
                                        IntProfilePicFileUrlId = obj.IntDocumentId,
                                        //IntAccountId = accountId,
                                        //IntBusinessUnitId = businessUnitId,
                                        IntCreatedBy = createdBy
                                    };
                                    _context.EmpEmployeePhotoIdentities.Add(newObj);
                                    _context.SaveChanges();
                                }
                                FileResponse response = new FileResponse
                                {
                                    globalFileUrlId = obj.IntDocumentId,
                                    fileName = file.FileName
                                };
                                responses.Add(response);
                            };
                        }
                        return Ok(responses);
                    }
                }
                else
                {
                    return BadRequest("Please provide EmployeeId");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        #endregion Upload Profile Picture

        #region FILE SERVER DATA TRANSFER

        [HttpGet]
        [Route("ContainerCreate")]
        public async Task<IActionResult> ContainerCreate(string containerName)
        {
            await client.CreateBlobContainerAsync(containerName);

            return Ok();
        }

        [HttpPost]
        [Route("UploadFileTEST")]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]         // Set the limit to 256 MB
        public async Task<IActionResult> UploadFileTEST(IList<IFormFile> files)
        {
            try
            {
                string filename = string.Empty;
                long maxFileSize = 1 * 1024 * 1024; // 1 MB
                List<FileValidationError> errors = new List<FileValidationError>();
                List<FileResponse> responses = new List<FileResponse>();

                foreach (var file in files)
                {
                    if (file.Length > maxFileSize)
                    {
                        long fileSize = file.Length / (1024 * 1024);
                        FileValidationError error = new FileValidationError
                        {
                            FileName = file.Name,
                            Message = "Maximum File Size Allowed (1MB) Your File Size (" + fileSize + "MB)"
                        };
                        errors.Add(error);
                    }
                }

                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }
                else
                {
                    foreach (var file in files)
                    {
                        BlobContainerClient container = client.GetBlobContainerClient("akijarl");

                        var blobClient = container.GetBlobClient(filename);

                        await blobClient.UploadAsync(file.OpenReadStream());

                        if (!string.IsNullOrEmpty(blobClient.Name))
                        {
                            FileResponse response = new FileResponse
                            {
                                fileName = blobClient.Name
                            };
                            responses.Add(response);
                        };
                    }
                    return Ok(responses);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("DownloadFileTEST")]
        public async Task<IActionResult> DownloadFileTEST(string id)
        {
            try
            {
                BlobContainerClient container = client.GetBlobContainerClient("akijarl");

                var blobClient = container.GetBlobClient(id);
                var downloadContent = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await downloadContent.Value.Content.CopyToAsync(ms);

                    return new FileContentResult(ms.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = Guid.NewGuid().ToString() + "application/octet-stream",
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw new Exception("internal server error");
            }
        }

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("DataTransfer")]
        //public async Task<IActionResult> DataTransfer()
        //{
        //    try
        //    {
        //        BlobContainerClient new_container = client.GetBlobContainerClient("saasclientfile");
        //        List<GlobalFileUrl> globalFileUrlList = await _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IsProcess == false).OrderBy(x => x.IntDocumentId).AsNoTracking().Take(2000).ToListAsync();

        //        foreach (GlobalFileUrl globalFileUrl in globalFileUrlList)
        //        {
        //            try
        //            {
        //                if (globalFileUrl != null)
        //                {
        //                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        //                    CloudBlobContainer container = blobClient.GetContainerReference("erpdata");

        //                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(globalFileUrl.StrFileServerId);
        //                    await blockBlob.FetchAttributesAsync();

        //                    string contentType = blockBlob.Properties.ContentType;

        //                    MemoryStream ms = new MemoryStream();
        //                    await blockBlob.DownloadToStreamAsync(ms);

        //                    //var file = File(ms, contentType, globalFileUrl.StrDocumentName);

        //                    IFormFile iFormFile = new FormFile(ms, 0, ms.Length, globalFileUrl.StrFileServerId, globalFileUrl.StrDocumentName);

        //                    // INSERT INTO NEW CONTAINER

        //                    var new_blobClient = new_container.GetBlobClient(globalFileUrl.StrFileServerId);

        //                    await new_blobClient.UploadAsync(iFormFile.OpenReadStream());

        //                    if (!string.IsNullOrEmpty(new_blobClient.Name))
        //                    {
        //                        //PeopleDesk.Data.Entity.saasclientfile.GlobalFileUrl obj = new PeopleDesk.Data.Entity.saasclientfile.GlobalFileUrl
        //                        //{
        //                        //    IntAccountId = globalFileUrl.IntAccountId,
        //                        //    IntBusinessUnitId = globalFileUrl.IntBusinessUnitId,
        //                        //    StrTableReferrence = globalFileUrl.StrTableReferrence,
        //                        //    StrRefferenceDescription = "Dcoument file for " + globalFileUrl.StrTableReferrence,
        //                        //    IntDocumentTypeId = globalFileUrl.IntDocumentTypeId,
        //                        //    StrFileServerId = new_blobClient.Name,
        //                        //    StrDocumentName = globalFileUrl.StrDocumentName,
        //                        //    NumFileSize = globalFileUrl.NumFileSize,
        //                        //    StrFileExtension = globalFileUrl.StrFileExtension,
        //                        //    StrServerLocation = "Azure File Server",
        //                        //    IntCreatedBy = globalFileUrl.IntCreatedBy,
        //                        //    DteCreatedAt = globalFileUrl.DteCreatedAt,
        //                        //};
        //                        //await _saasclientfile.GlobalFileUrls.AddAsync(obj);
        //                        //await _saasclientfile.SaveChangesAsync();
        //                    }
        //                }

        //                globalFileUrl.IsProcess = true;
        //                _context.GlobalFileUrls.Update(globalFileUrl);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }

        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error");
        //        throw new Exception("internal server error");
        //    }
        //}

        #endregion FILE SERVER DATA TRANSFER
    }
}