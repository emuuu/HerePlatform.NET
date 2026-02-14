using HerePlatformComponents;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

/// <summary>
/// Tests that EventArgs classes serialize/deserialize correctly via Helper (camelCase, null-omit).
/// This is critical because JS sends these as camelCase JSON via JSInterop.
/// </summary>
[TestFixture]
public class EventArgsSerializationTests
{
    #region MapPointerEventArgs

    [Test]
    public void MapPointerEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new MapPointerEventArgs
        {
            ViewportX = 400,
            ViewportY = 300,
            Position = new LatLngLiteral(52.52, 13.405),
            Button = 0,
            Buttons = 1,
            PointerType = "mouse",
            Type = "tap"
        };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"viewportX\":400"));
        Assert.That(json, Does.Contain("\"viewportY\":300"));
        Assert.That(json, Does.Contain("\"button\":0"));
        Assert.That(json, Does.Contain("\"buttons\":1"));
        Assert.That(json, Does.Contain("\"pointerType\":\"mouse\""));
        Assert.That(json, Does.Contain("\"type\":\"tap\""));
        Assert.That(json, Does.Contain("\"lat\":52.52"));
        Assert.That(json, Does.Contain("\"lng\":13.405"));
    }

    [Test]
    public void MapPointerEventArgs_Serialize_NullPosition_OmitsPosition()
    {
        var args = new MapPointerEventArgs
        {
            ViewportX = 100,
            ViewportY = 200,
            Type = "pointerdown"
        };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Not.Contain("\"position\""));
        Assert.That(json, Does.Contain("\"viewportX\":100"));
    }

    [Test]
    public void MapPointerEventArgs_Deserialize_FromJsJson()
    {
        // JSON as JS would send it (camelCase)
        var json = """{"viewportX":512,"viewportY":384,"position":{"lat":52.52,"lng":13.405},"button":2,"buttons":2,"pointerType":"mouse","type":"contextmenu"}""";

        var args = Helper.DeSerializeObject<MapPointerEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.ViewportX, Is.EqualTo(512));
        Assert.That(args.ViewportY, Is.EqualTo(384));
        Assert.That(args.Position, Is.Not.Null);
        Assert.That(args.Position!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(args.Position!.Value.Lng, Is.EqualTo(13.405));
        Assert.That(args.Button, Is.EqualTo(2));
        Assert.That(args.Buttons, Is.EqualTo(2));
        Assert.That(args.PointerType, Is.EqualTo("mouse"));
        Assert.That(args.Type, Is.EqualTo("contextmenu"));
    }

    [Test]
    public void MapPointerEventArgs_Deserialize_NullPosition()
    {
        var json = """{"viewportX":100,"viewportY":200,"position":null,"button":0,"buttons":0,"pointerType":"touch","type":"tap"}""";

        var args = Helper.DeSerializeObject<MapPointerEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Position, Is.Null);
        Assert.That(args.PointerType, Is.EqualTo("touch"));
    }

    [Test]
    public void MapPointerEventArgs_Deserialize_MissingOptionalFields()
    {
        // JS may omit null fields
        var json = """{"viewportX":100,"viewportY":200,"button":0,"buttons":0,"type":"tap"}""";

        var args = Helper.DeSerializeObject<MapPointerEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Position, Is.Null);
        Assert.That(args.PointerType, Is.Null);
        Assert.That(args.ViewportX, Is.EqualTo(100));
    }

    [Test]
    public void MapPointerEventArgs_RoundTrip_PreservesValues()
    {
        var original = new MapPointerEventArgs
        {
            ViewportX = 300.5,
            ViewportY = 200.75,
            Position = new LatLngLiteral(48.8566, 2.3522),
            Button = 1,
            Buttons = 4,
            PointerType = "pen",
            Type = "pointerdown"
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<MapPointerEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ViewportX, Is.EqualTo(original.ViewportX));
        Assert.That(result.ViewportY, Is.EqualTo(original.ViewportY));
        Assert.That(result.Position, Is.EqualTo(original.Position));
        Assert.That(result.Button, Is.EqualTo(original.Button));
        Assert.That(result.Buttons, Is.EqualTo(original.Buttons));
        Assert.That(result.PointerType, Is.EqualTo(original.PointerType));
        Assert.That(result.Type, Is.EqualTo(original.Type));
    }

    #endregion

    #region MapDragEventArgs

    [Test]
    public void MapDragEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new MapDragEventArgs
        {
            ViewportX = 250,
            ViewportY = 175,
            Position = new LatLngLiteral(52.52, 13.405),
            Type = "dragend"
        };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"viewportX\":250"));
        Assert.That(json, Does.Contain("\"viewportY\":175"));
        Assert.That(json, Does.Contain("\"type\":\"dragend\""));
        Assert.That(json, Does.Contain("\"lat\":52.52"));
    }

    [Test]
    public void MapDragEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"viewportX":300,"viewportY":400,"position":{"lat":51.5074,"lng":-0.1278},"type":"dragstart"}""";

        var args = Helper.DeSerializeObject<MapDragEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.ViewportX, Is.EqualTo(300));
        Assert.That(args.ViewportY, Is.EqualTo(400));
        Assert.That(args.Position!.Value.Lat, Is.EqualTo(51.5074));
        Assert.That(args.Position!.Value.Lng, Is.EqualTo(-0.1278));
        Assert.That(args.Type, Is.EqualTo("dragstart"));
    }

    [Test]
    public void MapDragEventArgs_Deserialize_NullPosition()
    {
        var json = """{"viewportX":100,"viewportY":200,"position":null,"type":"drag"}""";

        var args = Helper.DeSerializeObject<MapDragEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Position, Is.Null);
        Assert.That(args.Type, Is.EqualTo("drag"));
    }

    [Test]
    public void MapDragEventArgs_RoundTrip_PreservesValues()
    {
        var original = new MapDragEventArgs
        {
            ViewportX = 150.5,
            ViewportY = 250.75,
            Position = new LatLngLiteral(40.7128, -74.006),
            Type = "dragend"
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<MapDragEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ViewportX, Is.EqualTo(original.ViewportX));
        Assert.That(result.ViewportY, Is.EqualTo(original.ViewportY));
        Assert.That(result.Position, Is.EqualTo(original.Position));
        Assert.That(result.Type, Is.EqualTo(original.Type));
    }

    #endregion

    #region MapViewChangeEventArgs

    [Test]
    public void MapViewChangeEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new MapViewChangeEventArgs
        {
            Center = new LatLngLiteral(52.52, 13.405),
            Zoom = 14.0,
            Tilt = 30.0,
            Heading = 90.0,
            Type = "mapviewchangeend"
        };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"zoom\":14"));
        Assert.That(json, Does.Contain("\"tilt\":30"));
        Assert.That(json, Does.Contain("\"heading\":90"));
        Assert.That(json, Does.Contain("\"type\":\"mapviewchangeend\""));
    }

    [Test]
    public void MapViewChangeEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"center":{"lat":52.52,"lng":13.405},"zoom":12.5,"tilt":0,"heading":180,"type":"mapviewchange"}""";

        var args = Helper.DeSerializeObject<MapViewChangeEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Center!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(args.Center!.Value.Lng, Is.EqualTo(13.405));
        Assert.That(args.Zoom, Is.EqualTo(12.5));
        Assert.That(args.Tilt, Is.EqualTo(0));
        Assert.That(args.Heading, Is.EqualTo(180));
        Assert.That(args.Type, Is.EqualTo("mapviewchange"));
    }

    [Test]
    public void MapViewChangeEventArgs_Deserialize_NullCenter()
    {
        var json = """{"center":null,"zoom":10,"tilt":0,"heading":0,"type":"mapviewchangestart"}""";

        var args = Helper.DeSerializeObject<MapViewChangeEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Center, Is.Null);
        Assert.That(args.Zoom, Is.EqualTo(10));
    }

    [Test]
    public void MapViewChangeEventArgs_RoundTrip_PreservesValues()
    {
        var original = new MapViewChangeEventArgs
        {
            Center = new LatLngLiteral(35.6762, 139.6503),
            Zoom = 16.0,
            Tilt = 45.0,
            Heading = 270.0,
            Type = "mapviewchangeend"
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<MapViewChangeEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Center, Is.EqualTo(original.Center));
        Assert.That(result.Zoom, Is.EqualTo(original.Zoom));
        Assert.That(result.Tilt, Is.EqualTo(original.Tilt));
        Assert.That(result.Heading, Is.EqualTo(original.Heading));
        Assert.That(result.Type, Is.EqualTo(original.Type));
    }

    #endregion

    #region MapWheelEventArgs

    [Test]
    public void MapWheelEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new MapWheelEventArgs
        {
            Delta = -120,
            ViewportX = 512,
            ViewportY = 384
        };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"delta\":-120"));
        Assert.That(json, Does.Contain("\"viewportX\":512"));
        Assert.That(json, Does.Contain("\"viewportY\":384"));
    }

    [Test]
    public void MapWheelEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"delta":100,"viewportX":256,"viewportY":192}""";

        var args = Helper.DeSerializeObject<MapWheelEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Delta, Is.EqualTo(100));
        Assert.That(args.ViewportX, Is.EqualTo(256));
        Assert.That(args.ViewportY, Is.EqualTo(192));
    }

    [Test]
    public void MapWheelEventArgs_RoundTrip_PreservesValues()
    {
        var original = new MapWheelEventArgs
        {
            Delta = -53.5,
            ViewportX = 800.25,
            ViewportY = 600.75
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<MapWheelEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Delta, Is.EqualTo(original.Delta));
        Assert.That(result.ViewportX, Is.EqualTo(original.ViewportX));
        Assert.That(result.ViewportY, Is.EqualTo(original.ViewportY));
    }

    #endregion

    #region BaseLayerChangeEventArgs

    [Test]
    public void BaseLayerChangeEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new BaseLayerChangeEventArgs { Type = "baselayerchange" };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"type\":\"baselayerchange\""));
    }

    [Test]
    public void BaseLayerChangeEventArgs_Serialize_NullType_OmitsType()
    {
        var args = new BaseLayerChangeEventArgs();

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Not.Contain("\"type\""));
    }

    [Test]
    public void BaseLayerChangeEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"type":"baselayerchange"}""";

        var args = Helper.DeSerializeObject<BaseLayerChangeEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.Type, Is.EqualTo("baselayerchange"));
    }

    [Test]
    public void BaseLayerChangeEventArgs_RoundTrip_PreservesValues()
    {
        var original = new BaseLayerChangeEventArgs { Type = "baselayerchange" };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<BaseLayerChangeEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Type, Is.EqualTo(original.Type));
    }

    #endregion

    #region EngineStateChangeEventArgs

    [Test]
    public void EngineStateChangeEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new EngineStateChangeEventArgs { State = 2, Type = "enginestatechange" };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"state\":2"));
        Assert.That(json, Does.Contain("\"type\":\"enginestatechange\""));
    }

    [Test]
    public void EngineStateChangeEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"state":1,"type":"enginestatechange"}""";

        var args = Helper.DeSerializeObject<EngineStateChangeEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.State, Is.EqualTo(1));
        Assert.That(args.Type, Is.EqualTo("enginestatechange"));
    }

    [Test]
    public void EngineStateChangeEventArgs_RoundTrip_PreservesValues()
    {
        var original = new EngineStateChangeEventArgs { State = 3, Type = "enginestatechange" };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<EngineStateChangeEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.State, Is.EqualTo(original.State));
        Assert.That(result.Type, Is.EqualTo(original.Type));
    }

    #endregion

    #region StateChangeEventArgs

    [Test]
    public void StateChangeEventArgs_Serialize_ProducesCamelCaseJson()
    {
        var args = new StateChangeEventArgs { State = "open", Type = "statechange" };

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Contain("\"state\":\"open\""));
        Assert.That(json, Does.Contain("\"type\":\"statechange\""));
    }

    [Test]
    public void StateChangeEventArgs_Serialize_NullProperties_AreOmitted()
    {
        var args = new StateChangeEventArgs();

        var json = Helper.SerializeObject(args);

        Assert.That(json, Does.Not.Contain("\"state\""));
        Assert.That(json, Does.Not.Contain("\"type\""));
    }

    [Test]
    public void StateChangeEventArgs_Deserialize_FromJsJson()
    {
        var json = """{"state":"closed","type":"statechange"}""";

        var args = Helper.DeSerializeObject<StateChangeEventArgs>(json);

        Assert.That(args, Is.Not.Null);
        Assert.That(args!.State, Is.EqualTo("closed"));
        Assert.That(args.Type, Is.EqualTo("statechange"));
    }

    [Test]
    public void StateChangeEventArgs_RoundTrip_PreservesValues()
    {
        var original = new StateChangeEventArgs { State = "open", Type = "statechange" };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<StateChangeEventArgs>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.State, Is.EqualTo(original.State));
        Assert.That(result.Type, Is.EqualTo(original.Type));
    }

    #endregion
}
