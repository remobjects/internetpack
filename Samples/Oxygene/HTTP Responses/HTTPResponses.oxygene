<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
  <PropertyGroup>
    <RootNamespace>HTTPResponses</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>HTTPResponses</AssemblyName>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>Properties\App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{7F2AEC27-045A-4023-BF01-488C214D4A3D}</ProjectGuid>
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
      <DesignableClassName>HTTPResponses.MainForm</DesignableClassName>
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