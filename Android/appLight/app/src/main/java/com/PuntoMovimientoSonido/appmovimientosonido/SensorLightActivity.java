

package com.PuntoMovimientoSonido.appmovimientosonido;

import android.app.Activity;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;
import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.view.Menu;
import android.widget.ProgressBar;
import android.widget.TextView;


public class SensorLightActivity extends Activity implements
		SensorEventListener {

	private SensorManager mSensorManager;
	private Sensor LightSensor ;
	private TextView txtLight;
	private ImageView image;


	/**
	 * Called when the activity is starting. This is where most initialization
	 * should go: calling setContentView(int) to inflate the activity's UI,
	 * using findViewById(int) to programmatically interact with widgets in the
	 * UI, calling managedQuery(android.net.Uri, String[], String, String[],
	 * String) to retrieve cursors for data being displayed, etc.
	 * <p>
	 * You can call finish() from within this function, in which case
	 * onDestroy() will be immediately called without any of the rest of the
	 * activity lifecycle (onStart(), onResume(), onPause(), etc) executing.
	 *
	 *
	 * @param savedInstanceState
	 *            If the activity is being re-initialized after previously being
	 *            shut down then this Bundle contains the data it most recently
	 *            supplied in onSaveInstanceState(Bundle).
	 */

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_light);
		txtLight = (TextView) findViewById(R.id.txtLight);
		image = (ImageView) findViewById(R.id.imageStar);

		// Get an instance of the sensor service
		mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
		LightSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);

	}

	/**
	 * Called when sensor values have changed. The length and contents of the
	 * values array vary depending on which sensor is being monitored
	 * 
	 * Detects a change on the light sensor
	 * 
	 * @param event
	 *            Object SensorEvent that controls the values of the sensors
	 * 
	 */
	@Override
	public final void onSensorChanged(SensorEvent event) {
		if(event.sensor.getType() == Sensor.TYPE_LIGHT)
			txtLight.setText("LIGHT: " + event.values[0]);

		if(event.values[0] < 100){
			image.setImageDrawable(getResources().getDrawable(R.drawable.fondo));
		}
		else if(event.values[0] < 200){
			image.setImageDrawable(getResources().getDrawable(R.drawable.fondo1));
		}
		else if(event.values[0] < 300){
			image.setImageDrawable(getResources().getDrawable(R.drawable.fondo2));
		}
		else if(event.values[0] < 400){
			image.setImageDrawable(getResources().getDrawable(R.drawable.fondo3));
		}
		else if(event.values[0] > 400){
			image.setImageDrawable(getResources().getDrawable(R.drawable.fondo4));
		}

	}

	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) {
		// TODO Auto-generated method stub

	};

	@Override
	protected void onResume() {
		super.onResume(); // registro del listener
		mSensorManager.registerListener(this, LightSensor, SensorManager.SENSOR_DELAY_GAME);
	}

	@Override
	protected void onStop() { // anular el registro del listener
		mSensorManager.unregisterListener(this);
		super.onStop();
	}
}
