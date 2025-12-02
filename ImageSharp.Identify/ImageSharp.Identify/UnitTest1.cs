using SixLabors.ImageSharp;

namespace ImageSharp.Identify;

public sealed class UnitTest1
{
	[Fact]
	public async Task Load()
	{
		using var stream   = new MemoryStream(Resources.elephant);
		var metadata = await Image.LoadAsync(stream);
		Assert.NotEmpty(metadata.Frames);
	}

		
	[Fact]
	public async Task Identify()
	{
		using var stream   = new MemoryStream(Resources.elephant);
		var metadata = await Image.IdentifyAsync(stream);
		Assert.NotEmpty(metadata.FrameMetadataCollection);
	}
}