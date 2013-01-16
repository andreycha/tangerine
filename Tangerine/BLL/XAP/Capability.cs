using System;
using System.Collections.Generic;
using System.Linq;

namespace Tangerine.BLL
{
    public sealed class Capability
    {
        private static List<Capability> m_capabilities = new List<Capability>();

        public string Id { get; private set; }

        public string Description { get; set; }

        static Capability()
        {
            // WP7
            m_capabilities.Add(new Capability("ID_CAP_APPOINTMENTS", "Applications that access appointment data."));
            m_capabilities.Add(new Capability("ID_CAP_CAMERA", "Applications that use the camera capabilities."));
            m_capabilities.Add(new Capability("ID_CAP_CONTACTS", "Applications that access contact data."));
            m_capabilities.Add(new Capability("ID_CAP_GAMERSERVICES", "Applications that can interact with Xbox LIVE APIs."));
            m_capabilities.Add(new Capability("ID_CAP_IDENTITY_DEVICE", "Applications that use device-specific information such as a unique device ID, manufacturer name, or model name."));
            m_capabilities.Add(new Capability("ID_CAP_IDENTITY_USER", "Applications that use the anonymous LiveID to uniquely identify the user in an anonymous fashion."));
            m_capabilities.Add(new Capability("ID_CAP_ISV_CAMERA", "Applications that use the primary or front-facing camera."));
            m_capabilities.Add(new Capability("ID_CAP_LOCATION", "Applications with access to location services."));
            m_capabilities.Add(new Capability("ID_CAP_MAP", "Provides access to mapping functionality."));
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB", "Applications that can access media library."));
            m_capabilities.Add(new Capability("ID_CAP_MICROPHONE", "Applications that use the microphone."));
            m_capabilities.Add(new Capability("ID_CAP_NETWORKING", "Applications with access to network services."));
            m_capabilities.Add(new Capability("ID_CAP_PHONEDIALER", "Applications that can place phone calls."));
            m_capabilities.Add(new Capability("ID_CAP_PUSH_NOTIFICATION", "Applications that can receive push notifications from an Internet service."));
            m_capabilities.Add(new Capability("ID_CAP_SENSORS", "Applications that use the Windows Phone sensors."));
            m_capabilities.Add(new Capability("ID_CAP_WEBBROWSERCOMPONENT", "Applications with access to network services."));
            m_capabilities.Add(new Capability("ID_HW_FRONTCAMERA", "Applications that have features which require the front-facing camera."));
            // WP8
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB_AUDIO", "Provides read-only access to audio items, including lists of audio items and audio item properties."));
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB_VIDEO", "Provides read-only access to video items, including lists of video items, and video item properties."));
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB_PHOTO", "Provides read-only access to photos in the media library, and photo properties."));
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB_PLAYBACK", "Provides read/write access to media items that are currently playing."));
            m_capabilities.Add(new Capability("ID_CAP_NFC_PROXIMITY", "Provides access to Near Field Communication (NFC) services. Windows Phone 8 and newer."));
            m_capabilities.Add(new Capability("ID_CAP_REMOVABLE_STORAGE", "Provides access to data storage on an external storage component, such as an SD card."));
            m_capabilities.Add(new Capability("ID_CAP_RINGTONE_ADD", "Provides the ability to add ringtones to the phone."));
            m_capabilities.Add(new Capability("ID_CAP_SPEECH_RECOGNITION", "Provides access to speech recognition and text-to-speech (TTS) services."));
            m_capabilities.Add(new Capability("ID_CAP_VOIP", "Provides access to voice over IP (VoIP) calling services."));
            m_capabilities.Add(new Capability("ID_CAP_WALLET", "Provides access to interactions with Wallet such as saving, updating, and deleting deals, membership cards, and payment instruments."));
            m_capabilities.Add(new Capability("ID_CAP_WALLET_PAYMENTINSTRUMENTS", "Provides access to Wallet payment instruments such as credit and debit cards."));
            m_capabilities.Add(new Capability("ID_CAP_WALLET_SECUREELEMENT", "Provides access to a Wallet secure element for secure NFC transactions."));
            // WP8 functional
            m_capabilities.Add(new Capability("ID_FUNCCAP_EXTEND_MEM", "App receives the maximum memory limit by phone type: 180 MB on lower-memory phones; 380 MB on phones with > 1-GB memory."));
        }

        Capability(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public static Capability GetCapability(string id)
        {
            return m_capabilities.Where(xapCap => xapCap.Id == id).FirstOrDefault();
        }
    }
}
