using UnityEngine;
using UnityEngine.Tilemaps;

public class GimmickDoor : SignalReceiver
{
    [SerializeField] private Tilemap    tilemap;
    [SerializeField] private TileBase   doorTile;
    [SerializeField] private Vector3Int cellPosition;

    protected override void OnReceive(bool value)
    {
        tilemap.SetTile(cellPosition, value ? null : doorTile);
    }
}
