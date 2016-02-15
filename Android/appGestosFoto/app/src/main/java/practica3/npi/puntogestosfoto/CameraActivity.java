package practica3.npi.puntogestosfoto;

import android.app.Activity;
import android.content.Intent;
import android.hardware.Camera;
import android.net.Uri;
import android.os.Environment;
import android.os.Handler;
import android.os.Bundle;
import android.util.Log;
import android.widget.FrameLayout;
import android.widget.Toast;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

public class CameraActivity extends Activity {
    private static final String TAG = "@string/app_name";

    private Camera camera;
    private CameraPreview cameraPreview;
    private Camera.PictureCallback pictureCallback;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_camera);

        // Instancia de Camera
        camera = Camera.open();

        cameraPreview = new CameraPreview(this, camera);
        FrameLayout vistaPreviaCamara = (FrameLayout) findViewById(R.id.vistaPreviaCamara);
        vistaPreviaCamara.addView(cameraPreview);

        //Tomamos una foto a los 3 segundos
        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Tomo la foto
                camera.takePicture(null, null, pictureCallback);

                // Mostramos path
                String path = Environment.getExternalStoragePublicDirectory(
                        Environment.DIRECTORY_PICTURES) + "@string/app_name";
                Toast.makeText(CameraActivity.this, "Imagen guardada en: " + path, Toast.LENGTH_LONG).show();
            }
        }, 3000);

        pictureCallback = new Camera.PictureCallback() {

            @Override
            public void onPictureTaken(byte[] data, Camera camera) {
                // Creamos el archivo que alojará la imagen
                File pictureFile = getOutputMediaFile();
                if (pictureFile == null){
                    Log.d(TAG, "Error al crear el archivo media.");
                    return;
                }

                // Guardamos la imagen en el archivo
                try {
                    FileOutputStream fos = new FileOutputStream(pictureFile);
                    fos.write(data);
                    fos.close();
                } catch (FileNotFoundException e) {
                    Log.d(TAG, "Archivo no encontrado: " + e.getMessage());
                } catch (IOException e) {
                    Log.d(TAG, "Error accediendo al archivo: " + e.getMessage());
                }

                // Volvemos a la actividad principal al echar la foto
                Intent intent = new Intent(CameraActivity.this, MainActivity.class);
                startActivity(intent);

            }
        };
    }

    @Override
    public void onBackPressed() {
    }

    @Override
    protected void onPause() {
        super.onPause();
        releaseCamera();
    }

    private void releaseCamera(){
        if (camera != null){
            camera.release();
            camera = null;
        }
    }

    // **************************************************************************************
    // *********** El siguiente código es necesario para alojar fotos en la SD **************
    // **************************************************************************************

    // Creamos un archivo Uri para guardar la imagen
    private static Uri getOutputMediaFileUri(){
        return Uri.fromFile(getOutputMediaFile());
    }

    // Creamos un archivo para guardar la imagen
    private static File getOutputMediaFile(){
        // Fijamos el siguiente directorio de la SD para almacenar las fotos: "Pictures/PuntoGestosFoto"
        File mediaStorageDir = new File(Environment.getExternalStoragePublicDirectory(
                Environment.DIRECTORY_PICTURES), "@string/app_name");

        // Creamos el directorio anterior si no existe
        if (! mediaStorageDir.exists()){
            if (! mediaStorageDir.mkdirs()){
                Log.d(TAG, "Error al crear el directorio");
                return null;
            }
        }

        // Creamos el nombre del archivo multimedia
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
        File mediaFile;
        mediaFile = new File(mediaStorageDir.getPath() + File.separator +
                    "IMG_"+ timeStamp + ".jpg");

        return mediaFile;
    }
}
