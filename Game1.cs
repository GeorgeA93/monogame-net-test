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
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
        if (server != null)
        {
            server.Close();
        }
        if (client != null)
        {
            client.Close();
        }
        SteamClient.Shutdown();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        SteamClient.RunCallbacks();

        if (server == null)
        {
            if (SteamNetworkingUtils.Status == SteamNetworkingAvailability.Current)
            {
                Console.WriteLine("Connected to steam relay - starting server");
                server = SteamNetworkingSockets.CreateRelaySocket<Server>();
                Console.WriteLine("Started server");

                Console.WriteLine("Connecting to server");
                client = SteamNetworkingSockets.ConnectRelay<Client>(Steamworks.SteamClient.SteamId);
                //TODO: SteamNetworking.OnP2PConnectionFailed
            }
            else
            {
                Console.WriteLine("Not connected to relay - waiting to start server.");
            }
        }

        if (server != null)
        {
            server.Receive();
        }
        if (client != null)
        {
            client.Receive();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
