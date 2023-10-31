#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PeopleDesk/PeopleDesk.csproj", "PeopleDesk/"]
RUN dotnet restore "PeopleDesk/PeopleDesk.csproj"
COPY . .
WORKDIR "/src/PeopleDesk"
RUN dotnet build "PeopleDesk.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PeopleDesk.csproj" -c Release -o /app/publish

RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .



# <wkhtmltopdf>
# ENV PATH=/app;$PATH
# RUN apt-get update
# RUN apt-get install wget fontconfig libfreetype6 libx11-6 libxcb1 libxext6 libxrender1 xfonts-75dpi xfonts-base libjpeg62-turbo -y

# RUN wget http://security.debian.org/debian-security/pool/updates/main/o/openssl/libssl1.0.0_1.0.1t-1+deb8u12_amd64.deb
# RUN dpkg -i libssl1.0.0_1.0.1t-1+deb8u12_amd64.deb

# RUN wget http://ftp.us.debian.org/debian/pool/main/libp/libpng/libpng12-0_1.2.50-2+deb8u3_amd64.deb
# RUN dpkg -i libpng12-0_1.2.50-2+deb8u3_amd64.deb

# RUN wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.5/wkhtmltox_0.12.5-1.jessie_amd64.deb
# RUN dpkg -i wkhtmltox_0.12.5-1.jessie_amd64.deb

# RUN cp /usr/local/bin/wkhtmlto* /usr/bin/
# ----------------------------
# RUN apk add --no-cache \
#   libstdc++ \
#   libx11 \
#   libxrender \
#   libxext \
#   libssl1.1 \
#   ca-certificates \
#   fontconfig \
#   freetype \
#   ttf-dejavu \
#   ttf-droid \
#   ttf-freefont \
#   ttf-liberation \
#   ttf-ubuntu-font-family \
# && apk add --no-cache --virtual .build-deps \
#   msttcorefonts-installer \
# \
# # Install microsoft fonts
# && update-ms-fonts \
# && fc-cache -f \
# \
# # Clean up when done
# && rm -rf /tmp/* \
# && apk del .build-deps

# # Copy wkhtmltopdf files from docker-wkhtmltopdf image to app folder
# COPY --from=wkhtmltopdf /bin/libwkhtmltox.so /app/libwkhtmltox.so
# -----------------------------------------
RUN apt-get update

RUN apt-get install wget libgdiplus -y

# RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dll
# RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dylib
# RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.so


RUN wget https://github.com/rdvojmoc/DinkToPdf/blob/master/v0.12.4/64%20bit/libwkhtmltox.dll
RUN wget https://github.com/rdvojmoc/DinkToPdf/blob/master/v0.12.4/64%20bit/libwkhtmltox.dylib
RUN wget https://github.com/rdvojmoc/DinkToPdf/blob/master/v0.12.4/64%20bit/libwkhtmltox.so

# </wkhtmltopdf>


ENTRYPOINT ["dotnet", "PeopleDesk.dll"]