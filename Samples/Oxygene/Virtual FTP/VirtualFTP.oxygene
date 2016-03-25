<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
  <PropertyGroup>
    <RootNamespace>VirtualFTP</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>VirtualFTP</AssemblyName>
    <AllowGlobals>False</AllowGlobals>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>Properties\App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{71130CD9-002B-49BC-811A-1F172BFBAA28}</ProjectGuid>
    <TargetFrameworkVersion>v2.0</TargetFrameworkVersion>
    <DefaultUses />
    <StartupClass />
    <InternalAssemblyName />
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
    <Reference Include="RemObjects.InternetPack.VirtualFTP">
      <Private>True</Private>
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
    <Compile Include="Properties\AssemblyInfo.pas">
      <SubType>Code</SubType>
    </Compile>
    <Compile Include="Main.pas">
      <Subtype>Form</Subtype>
      <DesignableClassName>VirtualFTP.MainForm</DesignableClassName>
    </Compile>
    <Content Include="Properties\App.ico">
      <SubType>Content</SubType>
    </Content>
    <EmbeddedResource Include="Main.resx" />
  </ItemGroup>
  <ItemGroup>
    <Folder Include="Properties\" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Elements\RemObjects.Elements.Echoes.targets" />
  <PropertyGroup>
    <PreBuildEvent />
  </PropertyGroup>
</Project>