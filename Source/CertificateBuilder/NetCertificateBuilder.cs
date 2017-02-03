/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace RemObjects.InternetPack
{
	sealed class NetCertificateBuilder : CertificateBuilder
	{
		#region Interop parameter constants
		const Int32 XCN_CERT_NAME_STR_NONE = 0;
		const Int32 XCN_AT_KEYEXCHANGE = 1;
		const Int32 XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG = 2;
		const Int32 XCN_CRYPT_HASH_ALG_OID_GROUP_ID = 1;
		const Int32 XCN_CRYPT_OID_INFO_PUBKEY_ANY = 0;
		const Int32 XCN_CRYPT_STRING_BASE64 = 1;
		const Int32 XCN_ALGORITHM_FLAGS_NONE = 0;
		const Int32 XCN_CONTEXT_USER = 1;
		const Int32 XCN_ALLOW_UNTRUSTED = 2;
		const Int32 XCN_EXPORT_WITH_ROOT = 2;
		#endregion

		#region Private fields
		private readonly String fHashAlgorithm;
		#endregion

		public NetCertificateBuilder(String hashAlgorithm)
		{
			this.fHashAlgorithm = hashAlgorithm;
		}

		private Object CreateDistinguishedName(String subject)
		{
			// CX500DistinguishedName lDistinguishedName = new CX500DistinguishedName>();
			Type lDistinguishedNameType = Type.GetTypeFromProgID(@"X509Enrollment.CX500DistinguishedName");
			Object lDistinguishedName = Activator.CreateInstance(lDistinguishedNameType);

			//lDistinguishedName.Encode("CN=" + subject, X500NameFlags.XCN_CERT_NAME_STR_NONE);
			lDistinguishedNameType.InvokeMember("Encode", BindingFlags.InvokeMethod, null, lDistinguishedName, new Object[] { "CN=" + subject, XCN_CERT_NAME_STR_NONE });


			return lDistinguishedName;
		}

		private Object CreatePrivateKey()
		{
			// Create a new private key for the certificate
			//CX509PrivateKey lPrivateKey = new CX509PrivateKey();
			Type lPrivateKeyType = Type.GetTypeFromProgID("X509Enrollment.CX509PrivateKey");
			Object lPrivateKey = Activator.CreateInstance(lPrivateKeyType);

			//lPrivateKey.Length = 1024;
			lPrivateKeyType.InvokeMember("Length", BindingFlags.SetProperty, null, lPrivateKey, new Object[] { 1024 });

			//lPrivateKey.KeySpec = X509KeySpec.XCN_AT_KEYEXCHANGE; // use is not limited
			lPrivateKeyType.InvokeMember("KeySpec", BindingFlags.SetProperty, null, lPrivateKey, new Object[] { XCN_AT_KEYEXCHANGE });

			//lPrivateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
			lPrivateKeyType.InvokeMember("ExportPolicy", BindingFlags.SetProperty, null, lPrivateKey, new Object[] { XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG });

			//lPrivateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
			lPrivateKeyType.InvokeMember("ProviderName", BindingFlags.SetProperty, null, lPrivateKey, new Object[] { "Microsoft Base Cryptographic Provider v1.0" });

			//lPrivateKey.MachineContext = false;
			lPrivateKeyType.InvokeMember("MachineContext", BindingFlags.SetProperty, null, lPrivateKey, new Object[] { false });

			//lPrivateKey.Create();
			lPrivateKeyType.InvokeMember("Create", BindingFlags.InvokeMethod, null, lPrivateKey, new Object[] { });


			return lPrivateKey;
		}

		private Object CreateHashAlgorithm()
		{
			// Use the SHA512 hashing algorithm
			//CObjectId lHashAlgorithm = new CObjectId();
			Type lCObjectIdType = Type.GetTypeFromProgID("X509Enrollment.CObjectId");
			Object lHashAlgorithm = Activator.CreateInstance(lCObjectIdType);

			//lHashAlgorithm.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID, ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
			//								AlgorithmFlags.XCN_ALGORITHM_FLAGS_NONE, this.fHashAlgorithm);
			lCObjectIdType.InvokeMember("InitializeFromAlgorithmName", BindingFlags.InvokeMethod, null, lHashAlgorithm,
				new Object[] { XCN_CRYPT_HASH_ALG_OID_GROUP_ID, XCN_CRYPT_OID_INFO_PUBKEY_ANY, XCN_ALGORITHM_FLAGS_NONE, this.fHashAlgorithm });


			return lHashAlgorithm;
		}

		private Object CreateKeyUseSection(Boolean isServer)
		{
			// Extended key usage (more info at MSDN)
			//CObjectId lUsageExtension = new CObjectId();
			Type lCObjectIdType = Type.GetTypeFromProgID("X509Enrollment.CObjectId");
			Object lUsageExtension = Activator.CreateInstance(lCObjectIdType);

			//lUsageExtension.InitializeFromValue(isServer ? "1.3.6.1.5.5.7.3.1" : "1.3.6.1.5.5.7.3.2");
			lCObjectIdType.InvokeMember("InitializeFromValue", BindingFlags.InvokeMethod, null, lUsageExtension,
				new object[] { this.GetCertificatePurpose(isServer) });


			//CObjectIds lOidList = new CObjectIds();
			Type lCObjectIdsType = Type.GetTypeFromProgID("X509Enrollment.CObjectIds");
			Object lOidList = Activator.CreateInstance(lCObjectIdsType);

			//lOidList.Add(lUsageExtension);
			lCObjectIdsType.InvokeMember("Add", BindingFlags.InvokeMethod, null, lOidList, new Object[] { lUsageExtension });


			//CX509ExtensionEnhancedKeyUsage lKeyUsage = new CX509ExtensionEnhancedKeyUsage();
			Type lExtensionEnhancedKeyUsageType = Type.GetTypeFromProgID("X509Enrollment.CX509ExtensionEnhancedKeyUsage");
			Object lKeyUsage = Activator.CreateInstance(lExtensionEnhancedKeyUsageType);

			//lKeyUsage.InitializeEncode(lOidList);
			lExtensionEnhancedKeyUsageType.InvokeMember("InitializeEncode", BindingFlags.InvokeMethod, null, lKeyUsage, new Object[] { lOidList });


			return lKeyUsage;
		}

		private Object CreateCertificateRequest(Object subject, Object issuer, Object privateKey, Object keyUseSection, Object hashAlgorithm)
		{
			// Create the self signing request
			//CX509CertificateRequestCertificate lCertificateRequest = new CX509CertificateRequestCertificate();
			Type lCertificateRequestCertificateType = Type.GetTypeFromProgID("X509Enrollment.CX509CertificateRequestCertificate");
			Object lCertificateRequest = Activator.CreateInstance(lCertificateRequestCertificateType);

			//lCertificateRequest.InitializeFromPrivateKey(X509CertificateEnrollmentContext.XCN_CONTEXT_USER, lPrivateKey, "");
			lCertificateRequestCertificateType.InvokeMember("InitializeFromPrivateKey", BindingFlags.InvokeMethod, null, lCertificateRequest,
				new Object[] { XCN_CONTEXT_USER, privateKey, "" });

			//lCertificateRequest.Subject = lSubjectDN;
			lCertificateRequestCertificateType.InvokeMember("Subject", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { subject });

			//lCertificateRequest.Issuer = lIssuerDN;
			lCertificateRequestCertificateType.InvokeMember("Issuer", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { issuer });

			//lCertificateRequest.NotBefore = DateTime.Now;
			lCertificateRequestCertificateType.InvokeMember("NotBefore", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { this.GetCertificateStartDate() });

			//lCertificateRequest.NotAfter = DateTime.Now.AddYears(10);
			lCertificateRequestCertificateType.InvokeMember("NotAfter", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { this.GetCertificateEndDate() });

			//lCertificateRequest.X509Extensions.Add((CX509Extension)lKeyUsage);
			Object lExtensions = lCertificateRequestCertificateType.InvokeMember("X509Extensions", BindingFlags.GetProperty, null, lCertificateRequest, new Object[] { });
			lExtensions.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, lExtensions, new Object[] { keyUseSection });

			//lCertificateRequest.HashAlgorithm = lHashAlgorithm;
			lCertificateRequestCertificateType.InvokeMember("HashAlgorithm", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { hashAlgorithm });

			//lCertificateRequest.SuppressDefaults = true;
			lCertificateRequestCertificateType.InvokeMember("SuppressDefaults", BindingFlags.SetProperty, null, lCertificateRequest, new Object[] { true });

			//lCertificateRequest.Encode();
			lCertificateRequestCertificateType.InvokeMember("Encode", BindingFlags.InvokeMethod, null, lCertificateRequest, new Object[] { });


			return lCertificateRequest;
		}

		private X509Certificate2 EnrollCertificate(String subject, String password, Object certificateRequest)
		{
			// Final enrollment process
			//CX509Enrollment lEnroll = new CX509Enrollment();
			Type lEnrollmentType = Type.GetTypeFromProgID("X509Enrollment.CX509Enrollment");
			Object lEnroll = Activator.CreateInstance(lEnrollmentType);

			// Load the certificate
			//lEnroll.InitializeFromRequest(lRequestCertificate);
			lEnrollmentType.InvokeMember("InitializeFromRequest", BindingFlags.InvokeMethod, null, lEnroll, new Object[] { certificateRequest });

			// Add a friendly name
			//lEnroll.CertificateFriendlyName = subject;
			lEnrollmentType.InvokeMember("CertificateFriendlyName", BindingFlags.SetProperty, null, lEnroll, new Object[] { subject });

			// Output the request in base64 and install it back as the response
			//String csr = lEnroll.CreateRequest();
			//lEnroll.InstallResponse(InstallResponseRestrictionFlags.XCN_ALLOW_UNTRUSTED, csr, EncodingType.XCN_CRYPT_STRING_BASE64, password);
			Object csr = lEnrollmentType.InvokeMember("CreateRequest", BindingFlags.InvokeMethod, null, lEnroll, new Object[] { });
			lEnrollmentType.InvokeMember("InstallResponse", BindingFlags.InvokeMethod, null, lEnroll,
				new Object[] { XCN_ALLOW_UNTRUSTED, csr, XCN_CRYPT_STRING_BASE64, password });

			// Output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
			//String lCertificateBase64encoded = lEnroll.CreatePFX(password, PFXExportOptions.XCN_EXPORT_WITH_ROOT);
			String lCertificateBase64encoded = (String)lEnrollmentType.InvokeMember("CreatePFX", BindingFlags.InvokeMethod, null, lEnroll,
				new Object[] { password, XCN_EXPORT_WITH_ROOT });

			// Instantiate the certificate
			return new X509Certificate2(Convert.FromBase64String(lCertificateBase64encoded), password, X509KeyStorageFlags.Exportable);
		}

		public override Byte[] Export(String subject, String issuer, String password, Boolean isServer)
		{
			Object lSubjectName = this.CreateDistinguishedName(subject);
			Object lIssuerName = this.CreateDistinguishedName(issuer);

			Object lPrivateKey = this.CreatePrivateKey();
			Object lHashAlgorithm = this.CreateHashAlgorithm();
			Object lKeyUseSection = this.CreateKeyUseSection(isServer);

			Object lCertificateRequest = this.CreateCertificateRequest(lSubjectName, lIssuerName, lPrivateKey, lKeyUseSection, lHashAlgorithm);

			X509Certificate2 lCertificate = this.EnrollCertificate(subject, password, lCertificateRequest);

			return lCertificate.Export(X509ContentType.Pkcs12);
		}
	}
}