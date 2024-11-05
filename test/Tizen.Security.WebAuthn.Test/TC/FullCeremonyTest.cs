/*
 * Copyright (c) 2024 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tizen.System;
using Tizen.Multimedia.Vision;
using System.Threading;
using System.IO;
using System.Text;
using Tizen.Security.WebAuthn;
using static WebauthnTest.TestCommonData;

namespace WebauthnTest
{
    public class TSFullCeremonyTest : TestCaseBase
    {
        private const string TAG = "WebAuthn";
        private const string MC_QR_PATH = "/tmp/MC_QR.jpg";
        private const string GA_QR_PATH = "/tmp/GA_QR.jpg";

        public override string TestName => "TS full ceremony positive";

        public override (bool result, string message) Run()
        {
            try
            {
                // Check if supported
                Information.TryGetValue("http://tizen.org/feature/security.webauthn", out bool isWebauthnSupported);
                Information.TryGetValue("http://tizen.org/feature/network.bluetooth.le", out bool isBleSupported);
                Information.TryGetValue("http://tizen.org/feature/network.wifi", out bool isWifiSupported);
                Information.TryGetValue("http://tizen.org/feature/network.ethernet", out bool isEthernetSupported);
                Information.TryGetValue("http://tizen.org/feature/network.telephony", out bool isTelephonySupported);

                bool isSupported = isWebauthnSupported && isBleSupported && (isWifiSupported || isEthernetSupported || isTelephonySupported);
                if (!isSupported)
                    throw new NotSupportedException("feature not supported");

                /**
                 * PRE CONDITION
                 * Remove any existing qr code
                 * */
                if (File.Exists(MC_QR_PATH))
                    File.Delete(MC_QR_PATH);

                if (File.Exists(GA_QR_PATH))
                    File.Delete(GA_QR_PATH);

                /** TEST CODE */
                WauthnError mcResponseErrno = WauthnError.None;
                WauthnError mcLinkedDataErrno = WauthnError.None;
                var mcResponseSem = new SemaphoreSlim(0, 1);
                var mcLinkedDataSem = new SemaphoreSlim(0, 1);
                var linkedDeviceMutex = new SemaphoreSlim(1, 1);

                // Declare response data that will be validated later
                PubkeyCredAttestation receivedAttestation = null;
                HybridLinkedData receivedLinkedDevice = null;

                // Create Make Credential callbacks
                void GenerateQrCodeCallback(string qrContents, object obj)
                {
                    GenerateQr(qrContents, MC_QR_PATH);
                }
                void McResponseCallback(PubkeyCredAttestation attestation, WauthnError err, object obj)
                {
                    receivedAttestation = attestation;
                    receivedLinkedDevice = attestation?.LinkedDevice;
                    mcResponseErrno = err;
                    mcResponseSem.Release();
                }
                void McLinkedDataCallback(HybridLinkedData linkedDevice, WauthnError err, object obj)
                {
                    linkedDeviceMutex.Wait();
                    receivedLinkedDevice = linkedDevice;
                    linkedDeviceMutex.Release();

                    if (err != WauthnError.NoneAndWait)
                    {
                        mcLinkedDataErrno = err;
                        mcLinkedDataSem.Release();
                    }
                }
                var mcCallbacks = new MakeCredentialCallbacks(GenerateQrCodeCallback, McResponseCallback, McLinkedDataCallback, "somedata");

                // Create Make Credential data
                var clientData = new ClientData(Encoding.ASCII.GetBytes("{}"), HashAlgorithm.Sha256);

                const string RP_ID = "acme.com";
                var rp = new RelyingPartyEntity("Acme", RP_ID);
                var user = new UserEntity("user name", Encoding.ASCII.GetBytes("\x00\x01\x02\x03\x04\x05\x06\x07"), "user display name");
                var pubkeyCredCreationOptions = new PubkeyCredCreationOptions(
                    rp,
                    user,
                    new List<PubkeyCredParam>{new (PubkeyCredType.PublicKey, CoseAlgorithm.EcdsaP256WithSha256), new (PubkeyCredType.PublicKey, CoseAlgorithm.RsaSsaPkcs1V1_5WithSha256)},
                    180000,
                    null,
                    new AuthenticationSelectionCriteria(AuthenticatorAttachment.CrossPlatform, ResidentKeyRequirement.Required, true, UserVerificationRequirement.Required),
                    null,
                    AttestationPref.None,
                    null,
                    null,
                    null
                    );

                // Make the MakeCredential API call
                Authenticator.MakeCredential(clientData, pubkeyCredCreationOptions, mcCallbacks);

                // Wait until the response is received
                mcResponseSem.Wait();

                // Check attestation errors
                if (mcResponseErrno != WauthnError.None)
                    Assert.True(false, "Error returned by a MC response callback");

                // Start a second task that will call GetAssertion
                var gaTask = Task.Run(() =>
                {
                    WauthnError gaResponseErrno = WauthnError.None;
                    WauthnError gaLinkedDataErrno = WauthnError.None;
                    PubkeyCredAssertion receivedAssertion = null;
                    var gaResponseSem = new SemaphoreSlim(0, 1);
                    var gaLinkedDataSem = new SemaphoreSlim(0, 1);

                    // Create Get Assertion callbacks
                    void GenerateQrCodeCallback(string qrContents, object obj)
                    {
                        GenerateQr(qrContents, GA_QR_PATH);
                    }
                    void GaResponseCallback(PubkeyCredAssertion assertion, WauthnError err, object obj)
                    {
                        receivedAssertion = assertion;
                        gaResponseErrno = err;
                        gaResponseSem.Release();
                    }
                    void GaLinkedDataCallback(HybridLinkedData linkedDevice, WauthnError err, object obj)
                    {
                        linkedDeviceMutex.Wait();
                        receivedLinkedDevice = linkedDevice;
                        linkedDeviceMutex.Release();

                        if (err != WauthnError.NoneAndWait)
                        {
                            gaLinkedDataErrno = err;
                            gaLinkedDataSem.Release();
                        }
                    }
                    var gaCallbacks = new GetAssertionCallbacks(GenerateQrCodeCallback, GaResponseCallback, GaLinkedDataCallback, "somedata");

                    // Copy linked data in a case a linkedDataCallback from MC has already been called
                    linkedDeviceMutex.Wait();
                    var linkedDevice = receivedLinkedDevice;
                    linkedDeviceMutex.Release();

                    var pubkeyCredRequestOptions = new PubkeyCredRequestOptions(
                        180000,
                        RP_ID,
                        null,
                        UserVerificationRequirement.Required,
                        null,
                        AttestationPref.None,
                        null,
                        null,
                        linkedDevice
                        );

                    // Make the GetAssertion API call
                    Authenticator.GetAssertion(clientData, pubkeyCredRequestOptions, gaCallbacks);

                    gaResponseSem.Wait();

                    // Check assertion errors
                    if (gaResponseErrno != WauthnError.None)
                        Assert.True(false, "Error returned by a GA response callback");

                    // Wait for all GA linked data callbacks
                    gaLinkedDataSem.Wait();
                    if (gaLinkedDataErrno != WauthnError.None)
                        Assert.True(false, "Error returned by a GA linked data callback");

                    // Validate the received assertion
                    ValidateAssertion(receivedAssertion);
                });

                // Wait for all MC linked data callbacks
                mcLinkedDataSem.Wait();

                // Wait for the GetAssertion task to complete
                gaTask.GetAwaiter().GetResult();

                // Validate the received attestation
                if (mcLinkedDataErrno != WauthnError.None)
                    Assert.True(false, "Error returned by a MC linked data callback");

                ValidateAttestation(receivedAttestation);

                /**
                 * POST CONDITION
                 * */

                return (true, "");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        private static void GenerateQr(string qrContents, string path)
        {
            const int width = 300;
            const int height = 300;

            BarcodeImageFormat format = BarcodeImageFormat.Jpeg;
            var qrConfig = new QrConfiguration(QrMode.Utf8, ErrorCorrectionLevel.Low, 5);
            var imageConfig = new BarcodeImageConfiguration(width, height, path, format);

            BarcodeGenerator.GenerateImage(qrContents, qrConfig, imageConfig);
        }

        private static void ValidateAttestation(PubkeyCredAttestation attestation)
        {
            Assert.IsNotNull(attestation, "Attestation returned by a callback should not be null");

            var id = attestation.Id;
            Assert.IsNotNull(id, "Id should not be null");
            Assert.IsInstanceOf<byte[]>(id, "Should return an instance of byte[]");

            var type = attestation.Type;
            Assert.IsNotNull(type, "Type should not be null");
            Assert.IsInstanceOf<PubkeyCredType>(type, "Should return an instance of PubkeyCredType");

            var rawId = attestation.RawId;
            Assert.IsNotNull(rawId, "RawId should not be null");
            Assert.IsInstanceOf<byte[]>(rawId, "Should return an instance of byte[]");

            var response = attestation.Response;
            Assert.IsNotNull(response, "Response should not be null");
            Assert.IsInstanceOf<AuthenticatorAttestationResponse>(response, "Should return an instance of AuthenticatorAttestationResponse");

            var attachment = attestation.AuthenticatorAttachment;
            Assert.IsNotNull(attachment, "AuthenticatorAttachment should not be null");
            Assert.IsInstanceOf<AuthenticatorAttachment>(attachment, "Should return an instance of AuthenticatorAttachment");

            var extensions = attestation.Extensions;
            AssertNullOrType<IEnumerable<AuthenticationExtension>>(extensions, "Should return an instance of IEnumerable<AuthenticationExtension>");

            var linkedDevice = attestation.LinkedDevice;
            AssertNullOrType<HybridLinkedData>(linkedDevice, "Should return an instance of HybridLinkedData");

            // Check response
            var clientDataJson = response.ClientDataJson;
            Assert.IsNotNull(clientDataJson, "ClientDataJson should not be null");
            Assert.IsInstanceOf<byte[]>(clientDataJson, "Should return an instance of byte[]");

            var attestationObject = response.AttestationObject;
            Assert.IsNotNull(attestationObject, "AttestationObject should not be null");
            Assert.IsInstanceOf<byte[]>(attestationObject, "Should return an instance of byte[]");

            var transport = response.Transports;
            Assert.IsNotNull(transport, "Transports should not be null");
            Assert.IsInstanceOf<AuthenticatorTransport>(transport, "Should return an instance of AuthenticatorTransport");

            var authenticatorData = response.AuthenticatorData;
            Assert.IsNotNull(authenticatorData, "AuthenticatorData should not be null");
            Assert.IsInstanceOf<byte[]>(authenticatorData, "Should return an instance of byte[]");

            var subjectPubkeyInfo = response.SubjectPubkeyInfo;
            Assert.IsNotNull(subjectPubkeyInfo, "SubjectPubkeyInfo should not be null");
            Assert.IsInstanceOf<byte[]>(subjectPubkeyInfo, "Should return an instance of byte[]");

            var pubkeyAlg = response.PubkeyAlg;
            Assert.IsNotNull(pubkeyAlg, "PubkeyAlg should not be null");
            Assert.IsInstanceOf<CoseAlgorithm>(pubkeyAlg, "Should return an instance of CoseAlgorithm");
        }

        private static void ValidateAssertion(PubkeyCredAssertion assertion)
        {
            Assert.IsNotNull(assertion, "Assertion returned by a callback should not be null");

            var id = assertion.Id;
            Assert.IsNotNull(id, "Id should not be null");
            Assert.IsInstanceOf<byte[]>(id, "Should return an instance of byte[]");

            var type = assertion.Type;
            Assert.IsNotNull(type, "Type should not be null");
            Assert.IsInstanceOf<PubkeyCredType>(type, "Should return an instance of PubkeyCredType");

            var rawId = assertion.RawId;
            Assert.IsNotNull(rawId, "RawId should not be null");
            Assert.IsInstanceOf<byte[]>(rawId, "Should return an instance of byte[]");

            var response = assertion.Response;
            Assert.IsNotNull(response, "Response should not be null");
            Assert.IsInstanceOf<AuthenticatorAssertionResponse>(response, "Should return an instance of AuthenticatorAssertionResponse");

            var attachment = assertion.AuthenticatorAttachment;
            Assert.IsNotNull(attachment, "AuthenticatorAttachment should not be null");
            Assert.IsInstanceOf<AuthenticatorAttachment>(attachment, "Should return an instance of AuthenticatorAttachment");

            var extensions = assertion.Extensions;
            AssertNullOrType<IEnumerable<AuthenticationExtension>>(extensions, "Should return an instance of IEnumerable<AuthenticationExtension>");

            var linkedDevice = assertion.LinkedDevice;
            AssertNullOrType<HybridLinkedData>(linkedDevice, "Should return an instance of HybridLinkedData");

            // Check response
            var clientDataJson = response.ClientDataJson;
            Assert.IsNotNull(clientDataJson, "ClientDataJson should not be null");
            Assert.IsInstanceOf<byte[]>(clientDataJson, "Should return an instance of byte[]");

            var authenticatorData = response.AuthenticatorData;
            Assert.IsNotNull(authenticatorData, "AuthenticatorData should not be null");
            Assert.IsInstanceOf<byte[]>(authenticatorData, "Should return an instance of byte[]");

            var signature = response.Signature;
            Assert.IsNotNull(signature, "Signature should not be null");
            Assert.IsInstanceOf<byte[]>(signature, "Should return an instance of byte[]");

            var userHandle = response.UserHandle;
            AssertNullOrType<byte[]>(userHandle, "Should return an instance of byte[]");

            var attestationObject = response.AttestationObject;
            AssertNullOrType<byte[]>(attestationObject, "Should return an instance of byte[]");
        }

        private static void AssertNullOrType<T>(object obj, string message)
        {
            if (obj is not null)
                Assert.IsInstanceOf<T>(obj, message);
        }
    }
}