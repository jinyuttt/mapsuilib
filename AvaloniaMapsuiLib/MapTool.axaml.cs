
using Avalonia.Controls;
using BruTile.Predefined;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets.ScaleBar;
using Mapsui;
using BruTile;
using Mapsui.Extensions;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets;
using Mapsui.Tiling.Fetcher;
using static NetTopologySuite.Geometries.Utilities.GeometryMapper;
using Mapsui.Styles;
using Mapsui.Manipulations;
using Mapsui.Utilities;
using System.Security.Principal;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Rendering.Skia;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Layers;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using Mapsui.Nts.Extensions;
using Mapsui.Nts;
namespace AvaloniaMapsuiLib;

public partial class MapTool : UserControl
{
    public MapTool()
    {
        InitializeComponent();
        InitMap();
    }
    private const string V = @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)";
    Mapsui.UI.Avalonia.MapControl mapui;

    public string MapUri { get; set; } = "http://online3.map.bdimg.com/onlinelabel/?qt=tile&x=1&y=1&z=3&styles=pl&udt=20200727&scaler=1&p=1";

    public Attribution MapAttribution { get; set; } = new Attribution("© Map contributors", "https://www.openstreetmap.org/copyright");

    public void InitMap()
    {
        MapRenderer.RegisterWidgetRenderer(typeof(RulerWidget), new RulerWidgetRenderer());
        mapui = this.FindControl<Mapsui.UI.Avalonia.MapControl>("MapControl");

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", V);

        //var osmAttribution = new Attribution("© Map contributors", "https://www.openstreetmap.org/copyright");
        //  var osmSource = new HttpClientTileSource(httpClient, new GlobalSphericalMercator(), "http://online{s}.map.bdimg.com/onlinelabel/?qt=tile&x={x}&y={y}&z={z}&styles=pl&udt=20141103&scaler=1", new[] { "a", "b", "c" }, name: "OpenStreetMap", attribution: osmAttribution);

        //string url = "http://online3.map.bdimg.com/onlinelabel/?qt=tile&x=1&y=1&z=3&styles=pl&udt=20200727&scaler=1&p=1";
        var osmSource = new HttpClientTileSource(httpClient, new GlobalSphericalMercator(), MapUri);
        var osmLayer = new TileLayer(osmSource, dataFetchStrategy: new DataFetchStrategy()) { Name = "百度地图" };
        FontFamily = "Microsoft YaHei";
        mapui.FontFamily = this.FontFamily;
       //

        mapui.Map.Widgets.Enqueue(new ScaleBarWidget(mapui.Map) { TextAlignment = Mapsui.Widgets.Alignment.Center, HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center, VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top });
       // mapui.Map.Widgets.Enqueue(new Mapsui.Widgets.Zoom.ZoomInOutWidget { MarginX = 20, MarginY = 40 });
        mapui.Map.Widgets.Add(CreateMouseCoordinatesWidget(mapui.Map, Mapsui.Widgets.VerticalAlignment.Bottom, Mapsui.Widgets.HorizontalAlignment.Left));
        mapui.Map.Widgets.Add(CreateZoomInOutWidget(Orientation.Vertical, Mapsui.Widgets.VerticalAlignment.Top, Mapsui.Widgets.HorizontalAlignment.Left));
        //mapui.Map.Layers.Add(osmLayer);
        mapui.Map.Widgets.Add(new RulerWidget() { IsActive = true }); // Active on startup. You need to set this value from a button in our own application.
        mapui.Map.Widgets.Add(CreateButtonWidget("distan", Mapsui.Widgets.VerticalAlignment.Top, Mapsui.Widgets.HorizontalAlignment.Right, (s, e) =>
        {
            
            var button = Caster.TryCastOrThrow<ButtonWidget>(s);
            if (e.GestureType == GestureType.DoubleTap)
            {
                button.Text = "distan";
                foreach (var wig in mapui.Map.Widgets)
                {
                    var rul = wig as RulerWidget;
                    if (rul != null)
                    {
                        rul.IsActive = true;
                    }
                }
                mapui.Map.RefreshGraphics();
                return;
            }
            foreach (var wig in mapui.Map.Widgets)
            {
                var rul = wig as RulerWidget;
                if (rul != null)
                {
                    rul.IsActive = false;
                }
            }
            
            button.Text = "move";

            mapui.Map.RefreshGraphics();
        }));
      

        mapui.Map.Layers.Add(CreateBingTileLayer());
        mapui.Map.Layers.Add(CreateLayer());
        mapui.Map.Layers.Add(CreatePolygonLayer());
        mapui.Map.Layers.Add(CreateGeometryLayer());
    }

    public static MemoryLayer CreateLayer() => new() { Name = "Points with labels", Features = CreateLayerFeatures() };




    public static ILayer CreateGeometryLayer()
    {
        return new Layer("GeometryCollection")
        {
            DataSource = new MemoryProvider(CreateGeometries())
        };
    }

    public static IEnumerable<IFeature> CreateGeometries()
    {
        yield return new GeometryFeature
        {
            Geometry = new GeometryCollection(
            new Geometry[] {
                new Point(-2000000, 2000000),
                //new LineString(new[] {
                //    new Coordinate(0, 0),
                //    new Coordinate(0, 10000000),
                //    new Coordinate(10000000, 10000000),
                //    new Coordinate(10000000, 0),
                //}),
                //new LineString(new[] {
                //    new Coordinate(1000000, 1000000),
                //    new Coordinate(9000000, 1000000),
                //    new Coordinate(9000000, 9000000),
                //    new Coordinate(1000000, 9000000),
                //}),
                new Polygon(
                    new LinearRing(new[] {
                        new Coordinate(-10000000, 0),
                        new Coordinate(-15000000, 5000000),
                        new Coordinate(-10000000, 10000000),
                        new Coordinate(-5000000, 5000000),
                        new Coordinate(-10000000, 0)
                    }),
                    new[] {
                        new LinearRing(new[] {
                            new Coordinate(-10000000, 1000000),
                            new Coordinate(-6000000, 5000000),
                            new Coordinate(-10000000, 9000000),
                            new Coordinate(-14000000, 5000000),
                            new Coordinate(-10000000, 1000000)
                        })
                    }
                )
            }),
            Styles = new IStyle[]
            {
                new VectorStyle
                {
                    Line = new Pen(Color.Red, 3),
                    Fill = new Brush(Color.Blue),
                    Outline = new Pen(Color.Purple, 5),
                },
            }
        };
    }

    public static ILayer CreatePolygonLayer()
    {
        return new Layer("Polygons")
        {
            DataSource = new MemoryProvider(CreatePolygon().ToFeatures()),
            Style = new VectorStyle
            {
                Fill = new Brush(new Color(150, 150, 30, 128)),
                Outline = new Pen
                {
                    Color = Color.Orange,
                    Width = 2,
                    PenStyle = PenStyle.DashDotDot,
                    PenStrokeCap = PenStrokeCap.Round
                }
            }
        };
    }
    private static List<Polygon> CreatePolygon()
    {
        var result = new List<Polygon>();

        var polygon1 = new Polygon(
            new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 10000000),
                new Coordinate(10000000, 10000000),
                new Coordinate(10000000, 0),
                new Coordinate(0, 0)
            }),
            new[] {
                new LinearRing(new[] {
                    new Coordinate(1000000, 1000000),
                    new Coordinate(9000000, 1000000),
                    new Coordinate(9000000, 9000000),
                    new Coordinate(1000000, 9000000),
                    new Coordinate(1000000, 1000000)
                })
            });

        result.Add(polygon1);

        //var polygon2 = new Polygon(
        //    new LinearRing(new[] {
        //        new Coordinate(-10000000, 0),
        //        new Coordinate(-15000000, 5000000),
        //        new Coordinate(-10000000, 10000000),
        //        new Coordinate(-5000000, 5000000),
        //        new Coordinate(-10000000, 0)
        //    }),
        //    new[] {
        //        new LinearRing(new[] {
        //            new Coordinate(-10000000, 1000000),
        //            new Coordinate(-6000000, 5000000),
        //            new Coordinate(-10000000, 9000000),
        //            new Coordinate(-14000000, 5000000),
        //            new Coordinate(-10000000, 1000000)
        //        })
        //    });

        //result.Add(polygon2);

       


        return result;
    }
    private static TileLayer CreateBingTileLayer()
    {
        var apiKey = "Enter your api key here"; // Contact Microsoft about how to use this
        var tileSource = KnownTileSources.Create(KnownTileSource.BingHybrid, apiKey, BingHybrid.DefaultCache);
        return new TileLayer(tileSource, dataFetchStrategy: new DataFetchStrategy()) // DataFetchStrategy prefetches tiles from higher levels
        {
            Name = "Bing Aerial",
        };
    }


    private static List<IFeature> CreateLayerFeatures()
    {
        var lst = CreateFeatures();
        foreach(var f in lst)
        {
            var styles = CreateDiverseStyles();
            foreach(var sty in styles)
            {
                f.Styles.Add(sty);
            }
           
        }
        return lst;
    }

    private static List<IFeature> CreateFeatures() => [
     
        CreateFeatureWithRightAlignedStyle(),
       
    ];

    private static IEnumerable<IStyle> CreateDiverseStyles()
    {
        const int radius = 16;
        return
        [
            //new SymbolStyle {SymbolScale = 0.8, Offset = new Offset(0, 0), SymbolType = SymbolType.Rectangle},
            //new SymbolStyle {SymbolScale = 0.6, Offset = new Offset(radius, radius), SymbolType = SymbolType.Rectangle, Fill = new Brush(Color.Red)},
            //new SymbolStyle {SymbolScale = 1, Offset = new Offset(radius, -radius), SymbolType = SymbolType.Rectangle},
            //new SymbolStyle {SymbolScale = 1, Offset = new Offset(-radius, -radius), SymbolType = SymbolType.Rectangle},
            //new SymbolStyle {SymbolScale = 0.8, Offset = new Offset(0, 0)},
            //new SymbolStyle {SymbolScale = 1.2, Offset = new Offset(radius, 0)},
            //new SymbolStyle {SymbolScale = 1, Offset = new Offset(0, radius)},
            //new SymbolStyle {SymbolScale = 1, Offset = new Offset(radius, radius)},
            CreateBitmapStyle("embedded://AvaloniaMapsuiLib.Images.ic.png", 0.7),
            CreateBitmapStyle("embedded://AvaloniaMapsuiLib.Images.ic.png", 0.8),
            CreateBitmapStyle("embedded://AvaloniaMapsuiLib.Images.ic.png", 0.9),
            CreateBitmapStyle("embedded://AvaloniaMapsuiLib.Images.ic.png", 1.0),
        ];
    }

    private static ImageStyle CreateBitmapStyle(string embeddedResourcePath, double scale)
    {
        return new ImageStyle { Image = embeddedResourcePath, SymbolScale = scale, Offset = new Offset(0, 32) };
    }
    private static MouseMoveCoordinatesWidget CreateMouseCoordinatesWidget(Map map,
    VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
    {
        return new MouseMoveCoordinatesWidget()
        {
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            Margin = new MRect(20)
        };
    }

    private static ZoomInOutWidget CreateZoomInOutWidget(Orientation orientation,
      VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
    {
        return new ZoomInOutWidget
        {
            Orientation = orientation,
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            Margin = new MRect(20),
        };
    }
    private static ButtonWidget CreateButtonWidget(string text, VerticalAlignment verticalAlignment,
      HorizontalAlignment horizontalAlignment, EventHandler<WidgetEventArgs> tapped) => new()
      {
          Text = text,
          VerticalAlignment = verticalAlignment,
          HorizontalAlignment = horizontalAlignment,
          Margin = new MRect(30),
          Padding = new MRect(10, 8),
          CornerRadius = 8,
          BackColor = new Color(0, 123, 255),
          TextColor = Color.White,
          WithTappedEvent = tapped
      };

    private static PointFeature CreateFeatureWithRightAlignedStyle() => new(new MPoint(0, -2000000))
    {
         
        Styles = [new LabelStyle
        {
            Text = "Right Aligned",
            BackColor = new Brush(Color.Gray),
            HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Right,
        }]

    };
}