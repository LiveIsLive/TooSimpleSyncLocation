using System.Net.Http.Json;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;

namespace TooSimpleSyncLocation;

[Service]
public class SyncLocationForegroundService : Service, ILocationListener
{
	private LocationManager _LocationManager;
	protected LocationManager LocationManager
	{
		get
		{
			if (this._LocationManager == null)
				this._LocationManager = (LocationManager)this.GetSystemService(LocationService);
			return this._LocationManager;
		}
	}

	private const int NotificationId = 1001;
	private const string ChannelId = "sync_location_foreground_service_channel";

	public override IBinder OnBind(Intent intent) => null;

	public override void OnCreate()
	{
		base.OnCreate();
		StartForegroundService();
	}

	public async void OnLocationChanged(Location location)
	{
		System.Net.Http.HttpClient client = new();
		await client.PostAsJsonAsync("http://www.core-hosting.net/Location/Update", new { location.Latitude, location.Longitude });
	}

	public void OnProviderDisabled(string provider)
	{
		//throw new NotImplementedException();
	}

	public void OnProviderEnabled(string provider)
	{
		//throw new NotImplementedException();
	}

	public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
	{
		//throw new NotImplementedException();
	}

	public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
	{
		// 执行你的后台任务（如定时检查、网络请求等）
		var criteria = new Criteria
		{
			Accuracy = Accuracy.Fine,
			PowerRequirement = Power.Medium
		};

		var locationProvider = this.LocationManager.GetBestProvider(criteria, true);

		if (locationProvider != null)
		{
			this.LocationManager.RequestLocationUpdates(
				locationProvider,
				1000 * 60, // 最小时间间隔(毫秒)
				50,    // 最小距离变化(米)
				this);
		}
		return StartCommandResult.Sticky;
	}

	private void StartForegroundService()
	{
		// 创建通知渠道（Android 8.0+ 需要）
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			var channel = new NotificationChannel(
				ChannelId,
				"Sync Location Foreground Service Channel",
				NotificationImportance.Low
			);
			var manager = (NotificationManager)GetSystemService(NotificationService);
			manager.CreateNotificationChannel(channel);
		}

		// 创建通知
		var notification = new Notification.Builder(this, ChannelId)
			.SetContentTitle("定位信息上传守护进程运行中")
			.SetContentText("定位信息上传后台任务正在执行...")
			//.SetSmallIcon(Resource.Drawable.ic_notification) // 替换为你的图标
			.Build();

		// 启动前台服务（必须调用，否则会抛出异常）
		StartForeground(NotificationId, notification);
	}
}