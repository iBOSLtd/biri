namespace PeopleDesk.Models.Employee
{
    public class MenuListOfFoodCornerEditViewModel
    {
        public long IntAutoId { get; set; }
        public string? StrDayName { get; set; }
        public string? StrMenu { get; set; }
        public string? StrStatus { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }

    }

    public class MenuListOfFoodCornerEditCommonViewModel
    {
        public long updateBy { get; set; }
        public List<EditMenuListEditParameterViewModel> menuListViewModelObj { get; set; }
    }
    public class EditMenuListEditParameterViewModel
    {
        public long? id { get; set; }
        //public string? day { get; set; }
        public string? menu { get; set; }
    }
}
