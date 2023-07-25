using UnityEngine;

public class Game {

  public InputHandler inputHandler;
  public Camera camera;
  public string levelName;
  Level level;

  public void Init() {
    Reset();
  }

  public void Destroy() {
    if (level != null) {
      level.Destroy();
    }
  }

  private void Reset() {
    Destroy();
    level = new Level(camera, levelName);
  }

  public Main.State OnUpdate() {
    if (inputHandler.escPressed) {
      inputHandler.Reset();
      return new Main.MenuState { };
    }

    // Update for frame.
    bool finished;
    level.Update(out finished);
    if (finished) {
      inputHandler.Reset();
      return new Main.MenuState { fromLevel = level.name };
    }

    return null;
  }
}
