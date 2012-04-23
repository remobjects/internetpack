<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="3.5">
  <PropertyGroup>
    <RootNamespace>FtpSync</RootNamespace>
    <OutputType>Exe</OutputType>
    <AssemblyName>FtpSync</AssemblyName>
    <AllowGlobals>False</AllowGlobals>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{23407C77-DAB9-4100-A34D-3746A890944E}</ProjectGuid>
    <TargetFrameworkVersion>v2.0</TargetFrameworkVersion>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <DefineConstants>DEBUG;TRACE;</DefineConstants>
    <OutputPath>.\bin\Debug</OutputPath>
    <GeneratePDB>True</GeneratePDB>
    <GenerateMDB>True</GenerateMDB>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
    <OutputPath>.\bin\Release</OutputPath>
    <EnableAsserts>False</EnableAsserts>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="mscorlib">
    </Reference>
    <Reference Include="RemObjects.InternetPack">
    </Reference>
    <Reference Include="System">
    </Reference>
    <Reference Include="System.Data">
    </Reference>
    <Reference Include="System.Drawing">
    </Reference>
    <Reference Include="System.Windows.Forms">
    </Reference>
    <Reference Include="System.Xml">
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Content Include="App.ico" />
    <Compile Include="AssemblyInfo.pas" />
    <Compile Include="FtpSyncWorker.pas" />
    <Compile Include="Main.pas" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Chrome\RemObjects.Chrome.targets" />
</Project>