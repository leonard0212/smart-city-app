using SmartCity.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class DetectionFile : Entity<Guid>
    {

        public DetectionFile()
        {
            CreatedAt = DateTime.Now;
        }

        public DetectionFile(Guid documentId, Guid fileId)
        {
            CreatedAt = DateTime.Now;
            DetectionId = documentId;
            FileId = fileId;
        }


        public Detection Detection { get; set; }
        public Guid DetectionId { get; set; }
        public AppFile File { get; set; }
        public Guid FileId { get; set; }


    }
}
