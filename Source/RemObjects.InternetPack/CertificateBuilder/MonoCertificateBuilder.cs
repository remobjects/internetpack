/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;

namespace RemObjects.InternetPack
{
	sealed class MonoCertificateBuilder : CertificateBuilder
	{
		#region Interop parameter constants 
		private const String PKCS9LocalKeyId = "1.2.840.113549.1.9.21";
		#endregion

		#region Private fields
		private readonly IMonoSecurityTypeProvider fTypeProvider;
		private readonly String fHashAlgorithm;
		#endregion

		public MonoCertificateBuilder(IMonoSecurityTypeProvider typeProvider, String hashAlgorithm)
		{
			this.fTypeProvider = typeProvider;
			this.fHashAlgorithm = hashAlgorithm;
		}

		private Byte[] GenerateCertificateSerialNumber()
		{
			// Serial # (must be positive)
			Byte[] lSerialNumber = Guid.NewGuid().ToByteArray();
			if ((lSerialNumber[0] & 0x80) == 0x80)
			{
				lSerialNumber[0] -= 0x80;
			}

			return lSerialNumber;
		}

		private Object CreateDistinguishedName(String subject)
		{
			return "CN=" + subject;
		}

		private Object CreateSubjectKey()
		{
			// Subject key
			CspParameters lProviderParameters = new CspParameters();
			lProviderParameters.KeyContainerName = Guid.NewGuid().ToString();
			lProviderParameters.KeyNumber = (Int32)KeyNumber.Exchange;
			lProviderParameters.Flags = CspProviderFlags.UseDefaultKeyContainer;

			RSA lSubjectKey = new RSACryptoServiceProvider(lProviderParameters);

			return lSubjectKey;
		}

		private Object CreateKeyUseSection(Boolean isServer)
		{
			// Certificate purpose
			//Mono.Security.X509.Extensions.ExtendedKeyUsageExtension usageExtension = new Mono.Security.X509.Extensions.ExtendedKeyUsageExtension();
			Type lExtendedKeyUsageExtensionType = this.fTypeProvider.GetType("Mono.Security.X509.Extensions.ExtendedKeyUsageExtension");
			Object lKeyUsage = Activator.CreateInstance(lExtendedKeyUsageExtensionType);

			//usageExtension.KeyPurpose.Add(this.GetCertificatePurpose(isServer));
			Object lKeyPurpose = lExtendedKeyUsageExtensionType.InvokeMember("KeyPurpose", BindingFlags.GetProperty, null, lKeyUsage, new Object[] { });
			lKeyPurpose.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, lKeyPurpose, new Object[] { this.GetCertificatePurpose(isServer) });


			return lKeyUsage;
		}

		private Object PrepareCertificateBuilder(Object name, Object subjectKey, Object keyUseSection)
		{
			//Mono.Security.X509.X509CertificateBuilder certificateBuilder = new Mono.Security.X509.X509CertificateBuilder(3);
			Type lCertificateBuilderType = this.fTypeProvider.GetType("Mono.Security.X509.X509CertificateBuilder");
			Object lCertificateBuilder = Activator.CreateInstance(lCertificateBuilderType);

			//certificateBuilder.SerialNumber = this.GenerateCertificateSerialNumber();
			lCertificateBuilderType.InvokeMember("SerialNumber", BindingFlags.SetProperty, null, lCertificateBuilder,
				new Object[] { this.GenerateCertificateSerialNumber() });

			//certificateBuilder.IssuerName = lDistinguishedName;
			//certificateBuilder.SubjectName = lDistinguishedName;
			lCertificateBuilderType.InvokeMember("IssuerName", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { name });
			lCertificateBuilderType.InvokeMember("SubjectName", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { name });

			//certificateBuilder.NotBefore = this.GetCertificateStartDate();
			lCertificateBuilderType.InvokeMember("NotBefore", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { this.GetCertificateStartDate().ToLocalTime() });

			//certificateBuilder.NotAfter = this.GetCertificateEndDate();
			lCertificateBuilderType.InvokeMember("NotAfter", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { this.GetCertificateEndDate().ToLocalTime() });

			//certificateBuilder.SubjectPublicKey = subjectKey;
			lCertificateBuilderType.InvokeMember("SubjectPublicKey", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { subjectKey });

			//certificateBuilder.Extensions.Add(usageExtension);
			Object lExtensions = lCertificateBuilderType.InvokeMember("Extensions", BindingFlags.GetProperty, null, lCertificateBuilder, new Object[] { });
			lExtensions.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, lExtensions, new Object[] { keyUseSection });

			//certificateBuilder.Hash = this.fHashAlgorithm; // "SHA512" by default
			lCertificateBuilderType.InvokeMember("Hash", BindingFlags.SetProperty, null, lCertificateBuilder, new Object[] { this.fHashAlgorithm });


			return lCertificateBuilder;
		}

		private Byte[] BuildCertificate(Object builder, Object subjectKey, String password)
		{
			Type lCertificateBuilderType = builder.GetType();

			// Build the certificate
			//Byte[] rawData = certificateBuilder.Sign(subjectKey);
			Object lRawData = lCertificateBuilderType.InvokeMember("Sign", BindingFlags.InvokeMethod, null, builder, new Object[] { subjectKey });

			// Finalize the certificate

			// Avoid endianess issues 
			Hashtable lAttributes = new Hashtable(1) { { PKCS9LocalKeyId, new ArrayList { new Byte[] { 1, 0, 0, 0 } } } };

			//Mono.Security.X509.PKCS12 p12 = new Mono.Security.X509.PKCS12 { Password = "" };
			Type lPkcs12Type = this.fTypeProvider.GetType("Mono.Security.X509.PKCS12");
			Object p12 = Activator.CreateInstance(lPkcs12Type);
			lPkcs12Type.InvokeMember("Password", BindingFlags.SetProperty, null, p12, new Object[] { password });

			//p12.AddCertificate(new Mono.Security.X509.X509Certificate(rawData), attributes);
			Type lCertificateType = this.fTypeProvider.GetType("Mono.Security.X509.X509Certificate");
			Object lCertificate = Activator.CreateInstance(lCertificateType, lRawData);

			lPkcs12Type.InvokeMember("AddCertificate", BindingFlags.InvokeMethod, null, p12, new Object[] { lCertificate, lAttributes });

			//p12.AddPkcs8ShroudedKeyBag(subjectKey, attributes);
			lPkcs12Type.InvokeMember("AddPkcs8ShroudedKeyBag", BindingFlags.InvokeMethod, null, p12, new Object[] { subjectKey, lAttributes });

			return (Byte[])lPkcs12Type.InvokeMember("GetBytes", BindingFlags.InvokeMethod, null, p12, new Object[] { });
		}

		public override Byte[] Export(String subject, String password, Boolean isServer)
		{
			// Subject name
			Object lDistinguishedName = this.CreateDistinguishedName(subject);
			Object lSubjectKey = this.CreateSubjectKey();
			Object lKeyUseSection = this.CreateKeyUseSection(isServer);

			Object lCertificateBuilder = this.PrepareCertificateBuilder(lDistinguishedName, lSubjectKey, lKeyUseSection);

			return this.BuildCertificate(lCertificateBuilder, lSubjectKey, password);
		}
	}
}
