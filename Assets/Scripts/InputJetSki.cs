using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputJetSki : MonoBehaviour
{
    [SerializeField] private JetSki jetSki;

    [SerializeField] private string jetSkiTrottleUp;
    [SerializeField] private string jetSkiTrottleDown;
    [SerializeField] private string jetSkiLeft;
    [SerializeField] private string jetSkiRight;

    private Player player;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void OnEnable()
    {
        player.AddInputEventDelegate(jetSki.ThrottleUp, UpdateLoopType.Update, jetSkiTrottleUp);
        player.AddInputEventDelegate(jetSki.ThrottleDown, UpdateLoopType.Update, jetSkiTrottleDown);
        player.AddInputEventDelegate(jetSki.RudderLeft, UpdateLoopType.Update, jetSkiLeft);
        player.AddInputEventDelegate(jetSki.RudderRight, UpdateLoopType.Update, jetSkiRight);
    }

    private void OnDisable()
    {
        player.RemoveInputEventDelegate(jetSki.ThrottleUp);
        player.RemoveInputEventDelegate(jetSki.ThrottleDown);
        player.RemoveInputEventDelegate(jetSki.RudderLeft);
        player.RemoveInputEventDelegate(jetSki.RudderRight);
    }
}
