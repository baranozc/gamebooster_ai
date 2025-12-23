using Grpc.Core;
using GameBooster.Grpc;

namespace GameBooster.Grpc.Services
{
    public class BottleneckService : BottleneckCalculator.BottleneckCalculatorBase
    {
        public override Task<BottleneckReply> Calculate(BottleneckRequest request, ServerCallContext context)
        {
            // Basit bir darboğaz mantığı kuralım
            // Gerçek hayatta bu daha karmaşıktır ama proje için yeterli.
            
            double bottleneck = 0;
            string status = "Mükemmel Uyum";
            bool compatible = true;

            string gpu = request.GpuModel.ToUpper();
            string cpu = request.CpuModel.ToUpper();

            // Örnek Senaryo 1: Güçlü Kart + Zayıf İşlemci
            if ((gpu.Contains("4090") || gpu.Contains("4080")) && (cpu.Contains("I3") || cpu.Contains("RYZEN 3")))
            {
                bottleneck = 45.0;
                status = "KRİTİK DARBOĞAZ: İşlemci ekran kartını besleyemiyor!";
                compatible = false;
            }
            // Örnek Senaryo 2: Orta Kart + Orta İşlemci
            else if (gpu.Contains("4060") && cpu.Contains("I5"))
            {
                bottleneck = 5.0;
                status = "İyi Uyum";
            }
            // Varsayılan
            else
            {
                bottleneck = 10.0;
                status = "Kabul Edilebilir";
            }

            return Task.FromResult(new BottleneckReply
            {
                Percentage = bottleneck,
                Status = status,
                IsCompatible = compatible
            });
        }
    }
}