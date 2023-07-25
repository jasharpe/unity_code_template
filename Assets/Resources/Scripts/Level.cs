using UnityEngine;
using System.Collections.Generic;

public class Level {
  public string name;

  public Level(Camera camera, string levelName) {
    this.name = levelName;

    var rawLevel = Resources.Load<TextAsset>("Levels/" + levelName).text;
  }

  public void Destroy() {
  }

  public void Update(out bool finished) {
    finished = false;  // Set "finished" to false if the level should end.
  }
}