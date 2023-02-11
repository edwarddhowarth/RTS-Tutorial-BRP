using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Dynamically created when a skill is added to the skills list located on each Unit's scriptable object
 * Each skill creates its own SkillManager
 */
public class SkillManager : MonoBehaviour
{
    public SkillData skill;
    private GameObject _source;
    private Button _button;
    private bool _ready;

    public void Initialize(SkillData skill, GameObject source)
    {
        this.skill = skill;
        _source = source;
    }

    public void Trigger(GameObject target = null)
    {
        if (!_ready) return;
        StartCoroutine(_WrappedTrigger(target));
    }

    public void SetButton(Button button)
    {
        _button = button;
        _SetReady(true);
    }

    private IEnumerator _WrappedTrigger(GameObject target)
    {
        yield return new WaitForSeconds(skill.castTime);
        skill.Trigger(_source, target);
        _SetReady(false);
        yield return new WaitForSeconds(skill.cooldown);
        _SetReady(true);
    }

    private void _SetReady(bool ready)
    {
        _ready = ready;
        if (_button != null) _button.interactable = ready;
    }
}
