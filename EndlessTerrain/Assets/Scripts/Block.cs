using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block {

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

	public GrassBlock(int height, bool isVisible) : base(height, isVisible, 0f, 0.75f, 0f, 0.25f, 0.25f, 0.75f) {
		base.id = 1;
	}
}

public class ClearBlock : Block {

	public ClearBlock(int height, bool isVisible) : base(height, isVisible, 0f, 0.75f) {
		base.id = 0;
	}
}
