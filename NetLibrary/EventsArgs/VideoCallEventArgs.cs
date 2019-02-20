using NetLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.EventsArgs
{
    public class VideoCallEventArgs
    {
        public Guid TargetId { get; set; }

        public VideoCallEventArgs(Guid targetId)
        {
            TargetId = targetId;
        }
    }
}
