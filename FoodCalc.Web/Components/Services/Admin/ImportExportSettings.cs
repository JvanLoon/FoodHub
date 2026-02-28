namespace FoodCalc.Web.Components.Services.Admin;

public class ImportExportSettings
{
	public const long DefaultMaxFileSizeInBytes = 10 * 1024 * 1024;

	public long MaxFileSizeInBytes { get; set; } = DefaultMaxFileSizeInBytes;
}
