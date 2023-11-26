using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using DataIngress;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalcemPoMapie.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += MainWindow_KeyDown;

        _ = Task.Run(async () =>
        {
            IReadOnlyList<RawRegionData> result = await GeoJsonAdapter.ReadRegionDataFromFolder(@"C:\Users\AsyncMilk\Desktop\ShapeData");
            IList<RegionGeoData> regions = RegionGeoData.FromRawRegionData(result.ToList());
        });
    }

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
