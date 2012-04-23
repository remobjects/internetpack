<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="3.5">
  <PropertyGroup>
    <RootNamespace>SMTP Client Sample</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>SMTP Client Sample</AssemblyName>
    <AllowGlobals>False</AllowGlobals>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{CF645DB2-9A6A-45D1-ADBA-59FEF96B5103}</ProjectGuid>
    <TargetFrameworkVersion>v2.0</TargetFrameworkVersion>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <DefineConstants>DEBUG;TRACE;</DefineConstants>
    <OutputPath>.\bin\Debug</OutputPath>
    <GeneratePDB>True</GeneratePDB>
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
    <Compile Include="MainForm.pas">
      <Subtype>Form</Subtype>
      <DesignableClassName>SMTP_Client_Sample.MainForm</DesignableClassName>
    </Compile>
    <EmbeddedResource Include="MainForm.resx" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Chrome\RemObjects.Chrome.targets" />
</Project>