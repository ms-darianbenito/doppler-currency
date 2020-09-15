using System;

namespace Doppler.Currency.Dtos
{
    public class TrmResponse
    {
        public decimal Valor { get; set; }

        public DateTime VigenciaDesde { get; set; }

        public DateTime VigenciaHasta { get; set; }
    }
}
