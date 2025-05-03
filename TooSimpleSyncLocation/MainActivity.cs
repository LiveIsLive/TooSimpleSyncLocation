using Android;
using Android.Content.PM;
using Android.OS;

namespace TooSimpleSyncLocation;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Set our view from the "main" layout resource
		SetContentView(Resource.Layout.activity_main);

		this.CheckAndRequestLocationPermission();
	}



	const int RequestLocationId = 0;

	readonly string[] LocationPermissions =
	{
		Manifest.Permission.AccessCoarseLocation,
		Manifest.Permission.AccessFineLocation
	};

	void CheckAndRequestLocationPermission()
	{
		if ((int)Build.VERSION.SdkInt < 23)
			return;

		if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted)
			return;

		RequestPermissions(LocationPermissions, RequestLocationId);
	}

}