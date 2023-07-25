using UnityEngine;

public class Main : MonoBehaviour {

  private State state = null;

  private InputHandler inputHandler;

  abstract public class State {
    abstract public void TransitionFrom(Main main);

    abstract public void TransitionTo(Main main);

    abstract public State OnUpdate();
  }

  public class MenuState : State {
    public string fromLevel;
    private Menu menu;

    override public void TransitionFrom(Main main) {
      menu.Destroy();
    }

    override public void TransitionTo(Main main) {
      menu = new Menu { inputHandler = main.inputHandler };
      menu.Init(fromLevel);
    }

    override public State OnUpdate() {
      return menu.OnUpdate();
    }
  }

  public class GameState : State {
    public string levelName;
    private Game game;

    override public void TransitionFrom(Main main) {
      game.Destroy();
    }

    override public void TransitionTo(Main main) {
      game = new Game { camera = Camera.main, inputHandler = main.inputHandler, levelName = levelName };
      game.Init();
    }

    public override State OnUpdate() {
      return game.OnUpdate();
    }
  }

  private void Transition(State newState) {
    if (newState == null) {
      return;
    }
    if (state != null) {
      state.TransitionFrom(this);
    }
    newState.TransitionTo(this);
    state = newState;
  }

  protected void Start() {
    inputHandler = new InputHandler();
    Transition(new GameState { levelName = "1" });
  }

  protected void Update() {
    Transition(state.OnUpdate());
  }
}
