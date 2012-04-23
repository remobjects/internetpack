<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="3.5">
  <PropertyGroup>
    <RootNamespace>SampleServer</RootNamespace>
    <OutputType>WinExe</OutputType>
    <AssemblyName>SampleServer</AssemblyName>
    <AllowGlobals>False</AllowGlobals>
    <AllowLegacyCreate>False</AllowLegacyCreate>
    <ApplicationIcon>App.ico</ApplicationIcon>
    <Configuration Condition="'$(Configuration)' == ''">Release</Configuration>
    <ProjectGuid>{90E5FBCE-0445-40DB-BEC8-933B393487FE}</ProjectGuid>
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
    <Reference Include="mscorlib" />
    <Reference Include="RemObjects.InternetPack">
      <HintPath>$(ProgramFiles)\RemObjects Software\Internet Pack for .NET\Bin\RemObjects.InternetPack.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Data" />
    <Reference Include="System.Drawing" />
    <Reference Include="System.Windows.Forms" />
    <Reference Include="System.Xml" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="App.ico" />
    <Compile Include="AssemblyInfo.pas" />
    <Compile Include="Main.pas">
      <Subtype>Form</Subtype>
      <DesignableClassName>SampleServer.MainForm</DesignableClassName>
    </Compile>
    <EmbeddedResource Include="Main.resx" />
    <Content Include="HttpRoot\index.html">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="HttpRoot\ip.png">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="HttpRoot\Styles.css">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
  <ItemGroup>
    <Folder Include="HttpRoot\" />
    <Folder Include="Properties\" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath)\RemObjects Software\Chrome\RemObjects.Chrome.targets" />
</Project>