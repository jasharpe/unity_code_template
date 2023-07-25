﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SubMenu {
  public List<MenuOption> options;
  private int optionsIndex = 0;

  public void AddParents(Menu menu, SubMenu parent) {
    for (int i = 0; i < options.Count; i++) {
      if (options[i] is SubMenuOption) {
        ((SubMenuOption)options[i]).target.AddParents(menu, this);
      }
    }
    if (parent != null) {
      options.Add(new SubMenuOption {
        text = "Back",
        target = parent,
      });
    }
    for (int i = 0; i < options.Count; i++) {
      options[i].menu = menu;
    }
  }

  public void Show() {
    for (int i = 0; i < options.Count; i++) {
      options[i].Show(i, options.Count);
    }
    optionsIndex = 0;
    options[optionsIndex].Highlight();
  }

  public void Unshow() {
    for (int i = 0; i < options.Count; i++) {
      options[i].Unshow();
    }
  }

  public Main.State Enter() {
    return options[optionsIndex].Enter();
  }

  public void Down() {
    options[optionsIndex].Unhighlight();
    optionsIndex = (optionsIndex + 1) % options.Count;
    options[optionsIndex].Highlight();
  }

  public void Up() {
    options[optionsIndex].Unhighlight();
    optionsIndex = (optionsIndex - 1 + options.Count) % options.Count;
    options[optionsIndex].Highlight();
  }

  public void GoTo(int index) {
    if (index >= options.Count) {
      return;
    }
    options[optionsIndex].Unhighlight();
    optionsIndex = index;
    options[optionsIndex].Highlight();
  }
}

abstract public class MenuOption {
  public Menu menu;
  public string text;

  private GameObject gameObject;
  private TextMesh m;

  public void Show(int offset, int total) {
    gameObject = new GameObject("Menu Option: " + text);
    m = gameObject.AddComponent<TextMesh>();
    m.text = text;
    m.anchor = TextAnchor.MiddleCenter;
    m.fontSize = 120;
    m.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    m.transform.position = m.transform.position + new Vector3(0, -offset + total / 2f - 0.5f, 0);
    m.color = Color.black;
  }

  public void Unshow() {
    Object.Destroy(gameObject);
  }

  abstract public Main.State Enter();

  public void Highlight() {
    m.color = Color.green;
  }

  public void Unhighlight() {
    m.color = Color.black;
  }
}

public class SubMenuOption : MenuOption {
  public SubMenu target;

  override public Main.State Enter() {
    menu.GoTo(target);
    return null;
  }
}

public class LevelMenuOption : MenuOption {
  public string levelName;

  public override Main.State Enter() {
    return new Main.GameState{ levelName = levelName };
  }
}

public class ExitMenuOption : MenuOption {
  public override Main.State Enter() {
    Application.Quit();
    return null;
  }
}

public class Menu {
  public InputHandler inputHandler;

  private GameObject background;

  private SubMenu subMenu;

  public void Init(string fromLevel) {
    // TODO: Make sure this always covers the entire screen.
    background = new GameObject("Background");
    background.transform.localScale = new Vector3(30, 30, 1f);
    SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
    renderer.sortingOrder = -2;
    renderer.color = Color.white;
    renderer.sprite = Resources.Load<Sprite>("Sprites/Square");

    var levelMenuOptions = new List<MenuOption>();
    string levelsPath = "Assets/Resources/Levels/";
    DirectoryInfo dir = new DirectoryInfo(levelsPath);
    FileInfo[] infos = dir.GetFiles("*.txt");
    int fromLevelIndex = -1;
    for (int i = 0; i < infos.Length; i++) {
      var info = infos[i];
      var n = info.Name.Substring(0, info.Name.Length - 4);
      levelMenuOptions.Add(new LevelMenuOption {
        text = n,
        levelName = n,
      });
      if (n == fromLevel) {
        fromLevelIndex = i;
      }
    }
    var levelMenu = new SubMenu {
      options = levelMenuOptions,
    };

    subMenu = new SubMenu {
      options = new List<MenuOption>{
        new SubMenuOption {
          text = "Play",
          target = levelMenu,
        },
        new ExitMenuOption {
          text = "Exit",
        },
      },
    };
    subMenu.AddParents(this, null);
    if (fromLevelIndex != -1) {
      subMenu = levelMenu;
      levelMenu.Show();
      levelMenu.GoTo(fromLevelIndex + 1);
    } else {
      subMenu.Show();
    }
  }

  public void Destroy() {
    Object.Destroy(background);
    subMenu.Unshow();
  }

  public Main.State OnUpdate() {
    if (inputHandler.escPressed) {
      Debug.Log("Quit!");
      Application.Quit();
    }
    if (inputHandler.entPressed) {
      var newState = subMenu.Enter();
      if (newState != null) {
        inputHandler.Reset();
        return newState;
      }
    }
    if (inputHandler.downPressed) {
      subMenu.Down();
    }
    if (inputHandler.upPressed) {
      subMenu.Up();
    }
    inputHandler.Reset();
    return null;
  }

  public void GoTo(SubMenu subMenu) {
    this.subMenu.Unshow();
    this.subMenu = subMenu;
    this.subMenu.Show();
  }
}
