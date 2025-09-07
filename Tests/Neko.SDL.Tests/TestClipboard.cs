// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL;

namespace Neko.Sdl.Tests;

[TestFixture]
public class TestClipboard {
    [SetUp]
    public void SetUp() {
        Cleanups = 0;
        RequestedMimeTypes.Clear();
        NekoSDL.Init(InitFlags.Video);
    }

    [TearDown]
    public void TearDown() => NekoSDL.Quit();

    class ClipboardDataProvider : IClipboardDataProvider {
        public void CleanupClipboardData() {
            Cleanups++;
        }
        public byte[] GetClipboardData(string? mimeType) {
            if (mimeType is null)
                return [];
            RequestedMimeTypes.Enqueue(mimeType);
            return new byte[MyLength];
        }
        public string[] MimeTypes { get; } = ["test/one", "test/two"];
    }

    private ClipboardDataProvider _provider = new();

    [Test]
    public unsafe void TestClipboardData() {
        Clipboard.SetData(_provider);
        Assert.Multiple(() => {
            Assert.That(Clipboard.HasData("test/one"));
            Assert.That(Clipboard.HasData("test/two"));
            //Assert.That(!Clipboard.HasData("test/three"));
        });
        
        var data = Clipboard.GetData("test/one");

        try {
            Assert.That(data, Has.Length.EqualTo(MyLength));
            Assert.That(RequestedMimeTypes.Dequeue(), Is.EqualTo("test/one"));
        }
        finally
        {
            
        }

        Clipboard.Clear();

        Assert.That(Cleanups, Is.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(!SDL3.SDL_HasClipboardData("test/one"));
            Assert.That(!SDL3.SDL_HasClipboardData("test/two"));
            Assert.That(!SDL3.SDL_HasClipboardData("test/three"));
        });
        
        data = Clipboard.GetData("test/one");

        try {
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(RequestedMimeTypes, Has.Count.EqualTo(0));
        }
        finally
        {
        }
    }

    private static readonly Queue<string?> RequestedMimeTypes = [];
    private static int Cleanups;

    private const int MyLength = 17;
}