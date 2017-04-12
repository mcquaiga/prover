using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;

namespace Prover.Data.EF.Mappers
{
    public class SerializeConverter : ITypeConverter<object, string>
    {
        public string Convert(object source, string destination, ResolutionContext context)
        {
            return JsonConvert.SerializeObject(source);
        }
    }

    public class DeserializeConverter<T> : ITypeConverter<string, T>
    {     
        public T Convert(string source, T destination, ResolutionContext context)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
