using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks;

namespace SteamTest;


public class Server : SocketManager
{
    public override void OnConnecting(Steamworks.Data.Connection connection, Steamworks.Data.ConnectionInfo info)
    {
        base.OnConnecting(connection, info);
        Console.WriteLine("Client Connecting to server");
    }

    public override void OnConnected(Steamworks.Data.Connection connection, Steamworks.Data.ConnectionInfo info)
    {
        base.OnConnected(connection, info);
        Console.WriteLine("Client Connected to Server");
    }

    public override void OnDisconnected(Steamworks.Data.Connection connection, Steamworks.Data.ConnectionInfo info)
    {
        base.OnDisconnected(connection, info);
        Console.WriteLine("Client Disconnected from server");
    }
}

public class Client : ConnectionManager
{
    public override void OnConnected(Steamworks.Data.ConnectionInfo info)
    {
        base.OnConnected(info);
        Console.WriteLine("CLIENT CONNECT");
    }

    public override void OnConnecting(Steamworks.Data.ConnectionInfo info)
    {
        base.OnConnecting(info);
        Console.WriteLine("CLIENT CONNECTING");
    }

    public override void OnDisconnected(Steamworks.Data.ConnectionInfo info)
    {
        base.OnDisconnected(info);
        Console.WriteLine("CLIENT DISCONNECT");
    }
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Server server;
    private Client client;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        base.Initialize();

        SteamClient.Init(1442410, false);
        SteamNetworkingUtils.InitRelayNetworkAccess();
        server = SteamNetworkingSockets.CreateRelaySocket<Server>();
        client = SteamNetworkingSockets.ConnectRelay<Client>(Steamworks.SteamClient.SteamId);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        SteamClient.RunCallbacks();
        server.Receive();
        client.Receive();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
