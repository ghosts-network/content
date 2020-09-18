using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Api.Models
{
    public class UpdatePublicationModel
    {
        [Required]
        public string Content { get; set; }
    }
}
