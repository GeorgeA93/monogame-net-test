using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks;

namespace SteamTest;


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        base.Initialize();
        SteamAPI.Init();
        SteamNetworkingUtils.InitRelayNetworkAccess();

        Callback<SteamNetConnectionStatusChangedCallback_t>.Create((foo) =>
        {
            Console.WriteLine(foo.m_info.m_eState);
            SteamNetworkingSockets.AcceptConnection(foo.m_hConn);
        });

        SteamNetworkingSockets.CreateListenSocketP2P(0, 0, null);
        var id = SteamUser.GetSteamID();
        SteamNetworkingIdentity fo = new SteamNetworkingIdentity();
        fo.SetSteamID(id);
        SteamNetworkingSockets.ConnectP2P(ref fo,0, 0, null);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
        SteamAPI.Shutdown();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);

        SteamAPI.RunCallbacks();

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
