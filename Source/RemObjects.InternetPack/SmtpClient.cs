/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using RemObjects.InternetPack.CommandBased;
using RemObjects.InternetPack.Messages;

// http://www.ietf.org/rfc/rfc0821.txt
// http://www.faqs.org/rfcs/rfc2554.html 

namespace RemObjects.InternetPack.Email
{
#if DESIGN
    [System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Client), "Glyphs.SmtpClient.bmp")]
#endif
    public class SmtpClient : CommandBasedClient
    {
        public SmtpClient()
        {
            this.Port = 25;
        }

        public String HeloDomain
        {
            get
            {
                return this.fHeloDomain;
            }
            set
            {
                this.fHeloDomain = value;
            }
        }
        private String fHeloDomain;

        public Boolean UseAuth
        {
            get
            {
                return this.fUseAuth;
            }
            set
            {
                this.fUseAuth = value;
            }
        }
        private Boolean fUseAuth;

        public String AuthUser
        {
            get
            {
                return this.fAuthUser;
            }
            set
            {
                this.fAuthUser = value;
            }
        }
        private String fAuthUser;

        public String AuthPassword
        {
            get
            {
                return this.fAuthPassword;
            }
            set
            {
                this.fAuthPassword = value;
            }
        }
        private String fAuthPassword;

        public override void Open()
        {
            base.Open();

            if (!this.WaitForResponse(220))
            {
                this.Close();

                throw new Exception(String.Format("Invalid connection reply: {0} {1}", this.LastResponseNo, this.LastResponseText));
            }

            if (this.UseAuth)
            {
                if (!this.SendAndWaitForResponse("EHLO " + this.HeloDomain, 250))
                    throw new Exception(String.Format("Invalid ehlo reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

                if (!this.SendAndWaitForResponse("AUTH LOGIN", 334))
                    throw new Exception(String.Format("Invalid AUTH reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

                Byte[] lBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(this.AuthUser);
                String lEncodedString = System.Convert.ToBase64String(lBytes, 0, lBytes.Length);
                if (!SendAndWaitForResponse(lEncodedString, 334))
                    throw new Exception(String.Format("Invalid AUTH username reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

                lBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(this.AuthPassword);
                lEncodedString = System.Convert.ToBase64String(lBytes, 0, lBytes.Length);
                if (!this.SendAndWaitForResponse(lEncodedString, 235))
                    throw new Exception(String.Format("Invalid AUTH password reply: {0} {1}", this.LastResponseNo, this.LastResponseText));
            }
            else
            {
                // HELO moved to "Open" as it must only be sent once during session
                if (!this.SendAndWaitForResponse("HELO " + this.HeloDomain, 250))
                    throw new Exception(String.Format("Invalid helo reply: {0} {1}", this.LastResponseNo, this.LastResponseText));
            }
        }

        public override void Close()
        {
            if (this.Connected)
                this.SendAndWaitForResponse("QUIT", 221); // we don't care about it's result value, we are gonna quit anyway

            base.Close();
        }

        public void SendMessage(MailMessage message, String origin, String[] destination)
        {
            if (origin.Length == 0) throw
                new ArgumentException("No origin provided", "origin");

            if (destination.Length == 0)
                throw new ArgumentException("No destination provided", "destination");

            if (!this.SendAndWaitForResponse(String.Format("MAIL FROM: <{0}>", origin), 250))
                throw new Exception(String.Format("Invalid mail from reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

            for (Int32 i = 0; i < destination.Length; i++)
                if (!this.SendAndWaitForResponse(String.Format("RCPT TO: <{0}>", destination[i]), 250))
                    throw new Exception(String.Format("Invalid rcpt to reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

            if (!this.SendAndWaitForResponse("DATA", 354))
                throw new Exception(String.Format("Invalid data reply: {0} {1}", this.LastResponseNo, this.LastResponseText));

            MemoryStream lStr = new MemoryStream();

            message.EncodeMessage(lStr);
            lStr.Position = 0;
            lStr.WriteTo(this.CurrentConnection);
            if (!this.SendAndWaitForResponse(".", 250))
                throw new Exception(String.Format("Invalid data reply: {0} {1}", this.LastResponseNo, this.LastResponseText));
        }

        private static void EnlistMailAddresses(IList<String> destination, MessageAddresses addresses)
        {
            for (Int32 i = 0; i < addresses.Count; i++)
            {
                String lAddress = addresses[i].Address;

                if (String.IsNullOrEmpty(lAddress) || destination.Contains(lAddress))
                    continue;

                destination.Add(lAddress);
            }
        }

        public void SendMessage(MailMessage message)
        {
            List<String> lAddresses = new List<String>();

            SmtpClient.EnlistMailAddresses(lAddresses, message.To);
            SmtpClient.EnlistMailAddresses(lAddresses, message.Cc);
            SmtpClient.EnlistMailAddresses(lAddresses, message.Bcc);

            this.SendMessage(message, message.From.Address, lAddresses.ToArray());
        }
    }
}