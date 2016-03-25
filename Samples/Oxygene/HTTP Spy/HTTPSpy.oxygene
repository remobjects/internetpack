<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
  <PropertyGroup>
    <RootNamespace>HTTPSpy</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>HTTPSpy</AssemblyName>
    <AllowGlobals>False</AllowGlobals>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>Properties\App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{68032377-D8D8-4FEB-88F9-92658A6A56A2}</ProjectGuid>
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
      <HintPath>..\..\..\Bin\RemObjects.InternetPack.dll</HintPath>
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
    <Content Include="App.config">
      <SubType>Content</SubType>
    </Content>
    <Compile Include="MainForm.pas">
      <DesignableClassName>HTTPSpy.MainForm</DesignableClassName>
      <SubType>Form</SubType>
    </Compile>
    <Content Include="Properties\App.ico">
      <SubType>Content</SubType>
    </Content>
    <EmbeddedResource Include="MainForm.resx">
      <DependentUpon>MainForm.pas</DependentUpon>
    </EmbeddedResource>
    <Compile Include="Program.pas" />
  </ItemGroup>
  <ItemGroup>
    <Folder Include="Properties\" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Elements\RemObjects.Elements.Echoes.targets" />
  <PropertyGroup>
    <PreBuildEvent />
  </PropertyGroup>
</Project>