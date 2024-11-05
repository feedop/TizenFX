/*
 *  Copyright (c) 2024 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using Tizen.Security.WebAuthn;

namespace WebauthnTest
{
    internal static class TestCommonData
    {
        public static byte[] clientDataJson = Encoding.ASCII.GetBytes("{}");
        public static HashAlgorithm hashAlgorithm = HashAlgorithm.Sha256;
        public static ClientData clientData = new ClientData(clientDataJson, hashAlgorithm);

        public static object userData = "some data";
        public static MakeCredentialCallbacks mcCallbacks  = new MakeCredentialCallbacks(
            (string str, object obj) => {},
            (PubkeyCredAttestation attestation, WauthnError err, object obj) => {},
            (HybridLinkedData linkedData, WauthnError err, object obj) => {},
            userData);

        public static GetAssertionCallbacks gaCallbacks  = new GetAssertionCallbacks(
            (string str, object obj) => {},
            (PubkeyCredAssertion assertion, WauthnError err, object obj) => {},
            (HybridLinkedData linkedData, WauthnError err, object obj) => {},
            userData);

        public static byte[] attestationObject = new byte[16] {0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};
        public static byte[] authenticatorData = new byte[26] {0x21, 0x22, 0x23, 0x24, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] subjectPubkeyInfo = new byte[36] {0x31, 0x32, 0x33, 0x34, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static AuthenticatorTransport transports = AuthenticatorTransport.Ble;
        public static CoseAlgorithm pubkey_alg = CoseAlgorithm.EcdsaP256WithSha256;
        public static byte[] signature = new byte[26] {0x21, 0x22, 0x23, 0x24, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] userHandle = new byte[36] {0x31, 0x32, 0x33, 0x34, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};

        public static string name = "webauthn test";
        public static string id = "webauthn-test.com";
        public static RelyingPartyEntity relyingPartyEntity = new RelyingPartyEntity("webauthn test", "webauthn-test.com");

        public static byte[] bufferId = new byte[06] {0x01, 0x02, 0x03, 0x04, 0x0, 0x0};
        public static string displayName = "webauthn display name";
        public static UserEntity userEntity = new UserEntity(name, bufferId, displayName);

        public static PubkeyCredType pubkeyCredType = PubkeyCredType.PublicKey;
        public static PubkeyCredParam credParam0 = new PubkeyCredParam(pubkeyCredType, CoseAlgorithm.RsaSsaPkcs1V1_5WithSha256);
        public static PubkeyCredParam credParam1 = new PubkeyCredParam(pubkeyCredType, CoseAlgorithm.EcdsaP256WithSha256);
        public static IEnumerable<PubkeyCredParam> pubkeyCredParams2 = new List<PubkeyCredParam>{credParam0, credParam1};
        public static IEnumerable<PubkeyCredParam> pubkeyCredParams1 = new List<PubkeyCredParam>{credParam0};
        public static PubkeyCredDescriptor pubkeyCredDescriptor = new PubkeyCredDescriptor(pubkeyCredType, bufferId, transports);

        public static byte[] bufferId0 = new byte[06] {0x01, 0x02, 0x03, 0x04, 0x0, 0x0};
        public static byte[] bufferId1 = new byte[16] {0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static PubkeyCredDescriptor credDescriptor0 = new PubkeyCredDescriptor(pubkeyCredType, bufferId0, transports);
        public static PubkeyCredDescriptor credDescriptor1 = new PubkeyCredDescriptor(pubkeyCredType, bufferId0, AuthenticatorTransport.Smartcard);
        public static IEnumerable<PubkeyCredDescriptor> pubkeyCredDescriptors2 = new List<PubkeyCredDescriptor>{credDescriptor0, credDescriptor1};
        public static IEnumerable<PubkeyCredDescriptor> pubkeyCredDescriptors1 = new List<PubkeyCredDescriptor>{credDescriptor0};
        public static byte[] extensionId = new byte[06]{0x01, 0x02, 0x03, 0x04, 0x0, 0x0,};
        public static byte[] extensionValue = new byte[16]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static AuthenticationExtension authenticationExtension = new AuthenticationExtension(extensionId, extensionValue);

        public static byte[] extensionId1 = new byte[06]{0x01, 0x02, 0x03, 0x04, 0x0, 0x0,};
        public static byte[] extensionValue1 = new byte[16]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static AuthenticationExtension authenticationExtension1 = new AuthenticationExtension(extensionId1, extensionValue1);
        public static IEnumerable<AuthenticationExtension> authenticationExtensions1 = new List<AuthenticationExtension>{authenticationExtension};
        public static IEnumerable<AuthenticationExtension> authenticationExtensions2 = new List<AuthenticationExtension>{authenticationExtension, authenticationExtension1};

        public static AuthenticatorAttachment attachment = AuthenticatorAttachment.Platform;
        public static ResidentKeyRequirement residentKey = ResidentKeyRequirement.Discouraged;
        public static UserVerificationRequirement userVerification = UserVerificationRequirement.Discouraged;
        public static AuthenticationSelectionCriteria authenticatorSelCri = new AuthenticationSelectionCriteria(attachment, residentKey, false, userVerification);

        public static PubkeyCredHint hint0 = PubkeyCredHint.SecurityKey;
        public static PubkeyCredHint hint1 = PubkeyCredHint.ClientDevice;
        public static IEnumerable<PubkeyCredHint> pubkeyCredHints1 = new List<PubkeyCredHint>{hint0};
        public static IEnumerable<PubkeyCredHint> pubkeyCredHints2 = new List<PubkeyCredHint>{hint0, hint1};

        public static byte[] contactId = new byte[06]{0x01, 0x02, 0x03, 0x04, 0x0, 0x0};
        public static byte[] linkId = new byte[16]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] linkSecret = new byte[26]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] authenticatorPubkey = new byte[16]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] authenticatorName = new byte[26]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] tunnelServerDomain = new byte[36]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static byte[] identityKey = new byte[36]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static HybridLinkedData hybridLinkedData = new HybridLinkedData(
            contactId,
            linkId,
            linkSecret,
            authenticatorPubkey,
            authenticatorName,
            signature,
            tunnelServerDomain,
            identityKey);

        public static byte[] buffer0 = new byte[06]{0x01, 0x02, 0x03, 0x04, 0x0, 0x0};
        public static byte[] buffer1 = new byte[16]{0x11, 0x12, 0x13, 0x14, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,};
        public static IEnumerable<byte[]> attestationFormats1 = new List<byte[]>{buffer0};
        public static IEnumerable<byte[]> attestationFormats2 = new List<byte[]>{buffer0, buffer1};

        public static ulong timeout = 1000;
        public static AttestationPref attestation = AttestationPref.Direct;
        public static PubkeyCredCreationOptions pubkeyCredCreationOptions = new PubkeyCredCreationOptions(
            relyingPartyEntity,
            userEntity,
            pubkeyCredParams2,
            timeout,
            pubkeyCredDescriptors2,
            authenticatorSelCri,
            pubkeyCredHints2,
            attestation,
            attestationFormats1,
            authenticationExtensions2,
            hybridLinkedData);
        public static PubkeyCredCreationOptions pubkeyCredCreationOptionsNoLD = new PubkeyCredCreationOptions(
            relyingPartyEntity,
            userEntity,
            pubkeyCredParams2,
            timeout,
            pubkeyCredDescriptors2,
            authenticatorSelCri,
            pubkeyCredHints2,
            attestation,
            attestationFormats1,
            authenticationExtensions2,
            null);
        public static PubkeyCredCreationOptions emptyPubkeyCredCreationOptions = new PubkeyCredCreationOptions(
            null,
            null,
            null,
            0,
            null,
            null,
            null,
            AttestationPref.None,
            null,
            null,
            null);
        public static PubkeyCredCreationOptions pubkeyCredCreationOptionsOK = new PubkeyCredCreationOptions(
            relyingPartyEntity,
            userEntity,
            pubkeyCredParams2,
            0,
            null,
            null,
            null,
            AttestationPref.None,
            null,
            null,
            hybridLinkedData);
        public static PubkeyCredCreationOptions pubkeyCredCreationOptionsOKNoLD = new PubkeyCredCreationOptions(
            relyingPartyEntity,
            userEntity,
            pubkeyCredParams2,
            0,
            null,
            null,
            null,
            AttestationPref.None,
            null,
            null,
            null);


        public static string rpId = "test RP ID";
        public static PubkeyCredRequestOptions pubkeyCredRequestOptions = new PubkeyCredRequestOptions(
            timeout,
            rpId,
            pubkeyCredDescriptors2,
            userVerification,
            pubkeyCredHints2,
            attestation,
            attestationFormats1,
            authenticationExtensions2,
            hybridLinkedData);
        public static PubkeyCredRequestOptions pubkeyCredRequestOptionsNoLD = new PubkeyCredRequestOptions(
            timeout,
            rpId,
            pubkeyCredDescriptors2,
            userVerification,
            null,
            attestation,
            attestationFormats1,
            authenticationExtensions2,
            null);
        public static PubkeyCredRequestOptions emptyPubkeyCredRequestOptions = new PubkeyCredRequestOptions(
            0,
            null,
            null,
            UserVerificationRequirement.None,
            null,
            AttestationPref.None,
            null,
            null,
            null);
        public static PubkeyCredRequestOptions pubkeyCredRequestOptionsOK = new PubkeyCredRequestOptions(
            0,
            rpId,
            null,
            UserVerificationRequirement.None,
            null,
            AttestationPref.None,
            null,
            null,
            hybridLinkedData);
        public static PubkeyCredRequestOptions pubkeyCredRequestOptionsOKNoLD = new PubkeyCredRequestOptions(
            0,
            rpId,
            null,
            UserVerificationRequirement.None,
            null,
            AttestationPref.None,
            null,
            null,
            null);
    }
}