<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="3.5">
  <PropertyGroup>
    <RootNamespace>HTTPResponses</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>HTTPResponses</AssemblyName>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{7F2AEC27-045A-4023-BF01-488C214D4A3D}</ProjectGuid>
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
    <Compile Include="Main.pas">
      <Subtype>Form</Subtype>
      <DesignableClassName>HTTPResponses.MainForm</DesignableClassName>
    </Compile>
    <EmbeddedResource Include="Main.resx" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Chrome\RemObjects.Chrome.targets" />
</Project>