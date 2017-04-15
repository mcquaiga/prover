using System;
using AutoMapper;
using Prover.CommProtocol.Common.Models.Instrument;
using Prover.Shared.DTO.Instrument;

namespace Prover.Services.Mappings.Convertors
{
    public class InstrumentObjectConverter : ITypeConverter<InstrumentDto, IInstrument>
    {
        public IInstrument Convert(InstrumentDto source, IInstrument destination, ResolutionContext context)
        {
            //var type = Type.GetType(source.InstrumentFactory);

            //if (type != null && type.IsAssignableFrom(typeof(IInstrumentFactory)))
            //{
            //    var instrumentFactory = Activator.CreateInstance(type) as IInstrumentFactory;
            //    return instrumentFactory?.Create(source.ItemData).Result;
            //}

            return default(IInstrument);
        }
    }
}