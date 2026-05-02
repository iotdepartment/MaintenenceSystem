namespace MaintenenceSystem.Models
{
    public class GraficasMaquinadosViewModel
    {
        public List<AreaChartDto> Areas { get; set; }
        public List<StatusChartDto> Status { get; set; }

        public int TotalGeneradas { get; set; }
        public int TotalCerradas { get; set; }
    }
}