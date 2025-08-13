using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block {      //Datastructure for map generation, no mesh data

	public float ox;
	public float oy;
	public float tx;   //topOffset
	public float ty;
	public float bx;  //bottomOffset
	public float by;

	public int visible = 0;
	public int height;

	public int id;

	public Block (int _height, bool isVisible, float ox_, float oy_, float tx_, float ty_, float bx_, float by_) {
		height = _height;
		ox = ox_;
		oy = oy_;
		tx = tx_;
		ty = ty_;
		bx = bx_;
		by = by_;
		SetVisible (isVisible);
	}

	public Block (int _height, bool isVisible, float ox_, float oy_) {
		height = _height;
		ox = ox_;
		oy = oy_;
		tx = ox_;
		ty = oy_;
		bx = ox_;
		by = oy_;
		SetVisible (isVisible);
	}

	public void SetVisible(bool isVisible) {
		visible = (isVisible) ? 1 : 0;
	}
}

public class GrassBlock : Block {

	public GrassBlock(int height, bool isVisible) : base(height, isVisible, 0f, 0.375f, 0f, 0.125f, 0.125f, 0.375f) {
		base.id = 3;//0f, 0.75f, 0f, 0.25f, 0.25f, 0.75f
	}
}

public class DirtBlock : Block {

	public DirtBlock(int height, bool isVisible) : base(height, isVisible, 0.125f, 0.375f) {
		base.id = 2;
	}
}

public class StoneBlock : Block {

	public StoneBlock(int height, bool isVisible) : base(height, isVisible, 0.25f, 0.375f) {
		base.id = 1;
	}
}

public class SandBlock : Block {

	public SandBlock(int height, bool isVisible) : base(height, isVisible, 0.5f, 0f) {
		base.id = 4;
	}
}

public class CoalBlock : Block {

	public CoalBlock(int height, bool isVisible) : base(height, isVisible, 0.125f, 0.25f) {
		base.id = 5;
	}
}

public class IronOre : Block {

	public IronOre(int height, bool isVisible) : base(height, isVisible, 0f, 0.25f) {
		base.id = 4;
	}
}

public class DiamondOre : Block {

	public DiamondOre(int height, bool isVisible) : base(height, isVisible, 0.5f, 0.375f) {
		base.id = 5;
	}
}

public class WoodBlock : Block {

	public WoodBlock(int height, bool isVisible) : base(height, isVisible, 0.125f, 0f, 0.25f, 0f, 0.25f, 0f) {
		base.id = 5;
	}
}

public class LeafBlock : Block {

	public LeafBlock(int height, bool isVisible) : base(height, isVisible, 0.375f, 0f) {
		base.id = 5;
	}
}

public class BedRock : Block {

	public BedRock(int height, bool isVisible) : base(height, isVisible, 0.5f, 0.25f) {
		base.id = 100;
	}
}

public class ClearBlock : Block {

	public ClearBlock(int height, bool isVisible) : base(height, isVisible, 0f, 0.75f) {
		base.id = 0;
	}
}



public static class BlockInfo {

	public static Vector3 One(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 1 + pos.y, 0 + pos.z) - (Vector3.one / 2f), scale); //1
	}
	public static Vector3 Two(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (-1 + pos.x, 1 + pos.y, 0 + pos.z) - (Vector3.one / 2f), scale); //2
	}
	public static Vector3 Three(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (-1 + pos.x, 1 + pos.y, 1 + pos.z) - (Vector3.one / 2f), scale); //3
	}
	public static Vector3 Four(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (0 + pos.x, 1 + pos.y, 1 + pos.z) - (Vector3.one / 2f), scale); //4
	}
	public static Vector3 Five(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 0 + pos.y, 0 + pos.z) - (Vector3.one / 2f), scale); //5
	}
	public static Vector3 Six(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (-1 + pos.x, 0 + pos.y, 0 + pos.z) - (Vector3.one / 2f), scale); //6
	}
	public static Vector3 Seven(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (-1 + pos.x, 0 + pos.y, 1 + pos.z) - (Vector3.one / 2f), scale); //7
	}
	public static Vector3 Eight(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 0 + pos.y, 1 + pos.z) - (Vector3.one / 2f), scale); //8
	}

	public static Vector3[] UPNormals() {
		Vector3[] norms = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
		return norms;
	}
	public static Vector3[] DownNormals() {
		Vector3[] norms = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
		return norms;
	}
	public static Vector3[] LeftNormals() {
		Vector3[] norms = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
		return norms;
	}
	public static Vector3[] RightNormals() {
		Vector3[] norms = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
		return norms;
	}
	public static Vector3[] ForwardNormals() {
		Vector3[] norms = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
		return norms;
	}
	public static Vector3[] BackNormals() {
		Vector3[] norms = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
		return norms;
	}
}

