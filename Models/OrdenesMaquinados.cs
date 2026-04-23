namespace MaintenenceSystem.Models
{
    public class OrdenesMaquinados
    {
        public int ID { get; set; }
        public string? Folio { get; set; }
        public string? NumeroEmpleado { get; set; }
        public string? Requisitor { get; set; }
        public string? Area { get; set; }
        public string? Descripcion { get; set; }
        public string? Tipo { get; set; }
        public string? Prioridad { get; set; }
        public int? Cantidad { get; set; }
        public string? Responsable { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int? HorasLaboradas { get; set; }
        public string? Status { get; set; }
    }
}
