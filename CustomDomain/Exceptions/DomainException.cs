using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Exceptions
{
  
        // <summary>Biznes qaydası pozulanda Domain-in atdığı xəta.</summary>
        public class DomainException(string message) : Exception(message);
}
