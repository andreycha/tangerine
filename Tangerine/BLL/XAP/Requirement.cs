using System.Collections.Generic;
using System.Linq;

namespace Tangerine.BLL
{
    public sealed class Requirement
    {
        private static List<Requirement> m_requirements = new List<Requirement>();

        public string Id { get; private set; }

        public string Description { get; set; }

        static Requirement()
        {
            // WP7 requirements
            m_requirements.Add(new Requirement("ID_REQ_MEMORY_90", "Indicates that the app requires more than 90 MB of memory and is not suited for a lower-memory device."));
            // WP8 requirements
            m_requirements.Add(new Requirement("ID_REQ_MEMORY_300", "Indicates that the app requires more than 180 MB of memory and is not suited for a lower-memory device."));
            m_requirements.Add(new Requirement("ID_REQ_FRONTCAMERA", "Indicates that an app requires a front-facing camera to function correctly. Adding this requirement prevents the app from installing on a phone without a front-facing camera."));
            m_requirements.Add(new Requirement("ID_REQ_REARCAMERA", "Indicates that an app requires a back-facing camera to function correctly. Selecting this option prevents the app from installing on a phone without a back-facing camera."));
            m_requirements.Add(new Requirement("ID_REQ_NFC", "Indicates that an app requires a phone with a chip that enables Near Field Communication (NFC) to function correctly. Selecting this option prevents the app from installing on a phone without an NFC chip."));
            m_requirements.Add(new Requirement("ID_REQ_MAGNETOMETER", "Indicates that an app requires a phone that contains a compass to function correctly. Selecting this option prevents the app from installing on a phone that doesn’t have a compass."));
            m_requirements.Add(new Requirement("ID_REQ_GYROSCOPE", "Indicates that an app requires a phone that contains a gyroscope to function correctly. Selecting this option prevents the app from installing on a phone that doesn’t have a gyroscope."));
        }

        Requirement(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public static Requirement GetRequirement(string id)
        {
            return m_requirements.Where(req => req.Id == id).FirstOrDefault();
        }
    }
}
