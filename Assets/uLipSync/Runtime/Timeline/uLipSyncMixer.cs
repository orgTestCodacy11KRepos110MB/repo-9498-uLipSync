using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace uLipSync.Timeline
{

public class uLipSyncMixer : PlayableBehaviour
{
    public uLipSyncTrack track { get; set; }
    public TimelineClip[] clips { get; set; }
    uLipSyncTimelineEvent _target = null;

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_target) _target.OnStop();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _target = playerData as uLipSyncTimelineEvent;
        if (!_target) return;

        var frame = BakedFrame.zero;

        for (int i = 0; i < clips.Length; i++)
        {
            var clip = clips[i];
            var asset = clips[i].asset as uLipSyncClip;
            var behaviour = asset.behaviour;
            var weight = playable.GetInputWeight(i);

            frame.volume += behaviour.frame.volume * asset.volume * weight;
            foreach (var phoneme in behaviour.frame.phonemes)
            {
                frame.phonemes.Add(new BakedPhonemeRatio() {
                    phoneme = phoneme.phoneme,
                    ratio = phoneme.ratio * weight,
                });
            }
        }

        _target.OnFrame(frame);
    }
}

}