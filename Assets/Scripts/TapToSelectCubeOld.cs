using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;
#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
using System;
#endif

public class TapToSelectCubeOld : MonoBehaviour {

	bool selecting = false;
	public Text countingTextList;

	// Called by GazeGestureManager when the user performs a Select gesture
	void OnSelect()
	{
		// On each Select gesture, toggle whether the user is in placing mode.
		selecting = !selecting;
		countingTextList = GameObject.FindGameObjectWithTag("TextPanel").GetComponent<Text>();

		// If the user is in placing mode, display the spatial mapping mesh.
		if (selecting)
		{
			countingTextList.text = "Ready!";
		}
		// If the user is not in placing mode, hide the spatial mapping mesh.
		else
		{
			countingTextList.text = "";
		}

	}

	// Update is called once per frame
	void Update()
	{
		// If the user is in placing mode,
		// update the placement to match the user's gaze.

		if (selecting)
		{
			countingTextList.text = "";
			// Do a raycast into the world that will only hit the Spatial Mapping mesh.
			var headPosition = Camera.main.transform.position;
			var gazeDirection = Camera.main.transform.forward;

			countingTextList.transform.position = new Vector3(countingTextList.transform.position.x, countingTextList.transform.position.y - (Camera.main.transform.forward.y*1.5f), countingTextList.transform.position.z);
//			float smooth = 2.0F;
//			float tiltAngle = 30.0F;
//			float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
//       		float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
//     		Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
//        	countingTextList.transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
			countingTextList.transform.rotation = Quaternion.Euler(0,0,0);
			RaycastHit hitInfo;
			if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
				30.0f, SpatialMapping.PhysicsRaycastMask))
			{
			try{
				//#if WINDOWS_UWP
				//Task task = new Task(
				//	async () =>
				//	{
				//	var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://Data/"+this.name));
				//	await FileStream

//Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
//Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("sample.txt");
////string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
//var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
//ulong size = stream.Size;
//using (var inputStream = stream.GetInputStreamAt(0))
//{
//    // We'll add more code here in the next step.
//	using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
//	{
//    	uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
//    	string text = dataReader.ReadString(numBytesLoaded);
//	}
//}

				var fileStream = new FileStream(@"c:\moose\" + this.name, FileMode.Open, FileAccess.Read);
				using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
				{
					//int i = 0;
					string line;
					bool isComment = false;
					while ((line = streamReader.ReadLine()) != null /*&& i < 100 */&& countingTextList.text.Length < 10000)
					{
						// i++;
						bool skip = false;
						if (line.IndexOf("/*") > -1)
						{
							isComment = true;
						}
						if (line.IndexOf("import") > -1 || line.IndexOf("//") > -1 || line.IndexOf("package") > -1 || line.Length == 0)
						{
							skip = true;
						}
						if (!isComment && !skip)
						{
							countingTextList.text += line + "\n";
						}
						if (line.IndexOf("*/") > -1)
						{
							isComment = false;
						}
					}

				}
				//countingTextList.text = this.transform.position.ToString();
			}
			catch(Exception e){
			}
		}
		}
	}
}
