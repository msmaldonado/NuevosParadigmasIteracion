/**
 * Copyright 2016
 * @author Cristina Zuheros Montes
 * @author Miguel Sánchez Maldonado
 * @version 10.02.2016
 * This file is part of appGPSVoz.
 *
 * appBrujulaVoz is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * appBrujulaVoz is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with appBrujulaVoz.  If not, see <http://www.gnu.org/licenses/>.
 */
package com.BrujulaVoz.appbrujulavoz;

import com.NPI.appbrujulavoz.R;

import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.app.Activity;
import android.content.Intent;

/**
 * Class MainActivity.
 * Controls the exectuion of the APP. Launches interface.
 *
 */
public class MainActivity extends Activity implements OnClickListener {

	/**
	 * Called when the activity is starting. This is where most initialization
	 * should go: calling setContentView(int) to inflate the activity's UI,
	 * using findViewById(int) to programmatically interact with widgets in the
	 * UI, calling managedQuery(android.net.Uri, String[], String, String[],
	 * String) to retrieve cursors for data being displayed, etc.
	 * <p>
	 *
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
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		Button startButton = (Button) findViewById(R.id.button_start);
		startButton.setOnClickListener(this);
	}

	/**
	 * Called when a view has been clicked.
	 * 
	 * @param view
	 *            The view that was clicked.
	 */
	@Override
	public void onClick(View v) {
		if (v.getId() == R.id.button_start) {
			Intent voiceActivity = new Intent(this, VoiceRecognitionActivity.class);
			startActivityForResult(voiceActivity, 1);
		}
	}

	/**
	 * Handle the results from the voice recognition activity.
	 */
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == 1) {
			if (resultCode == RESULT_OK) {
				String cardinal = data.getStringExtra("cardinal");
				float error = data.getFloatExtra("error", 0F);
				if (cardinal != null && error != -1){
					startCompass(cardinal, error);
				}
			}
			if (resultCode == RESULT_CANCELED) {
				// Write your code if there's no result
			}
		} else if (requestCode == 2) {
			if (resultCode == RESULT_OK) {
				TextView result = (TextView) findViewById(R.id.result);
				result.setText(R.string.result);
			}
			if (resultCode == RESULT_CANCELED) {
				// Write your code if there's no result
			}
		}
	}

	/**
	 * Iniciates the navigation to the exact point.
	 * 
	 * @param latitud
	 *            Latitude of the point
	 * 
	 * @param longitud
	 *            Lenght of the point
	 *
	 */
	public void startCompass(String cardinal, float error) {
		Intent GPSActivity = new Intent(this, CompassActivity.class);
		GPSActivity.putExtra("cardinal", cardinal);
        GPSActivity.putExtra("error", error);
		startActivityForResult(GPSActivity, 2);
	}

}