using System.Collections.Generic;
using System.Linq;
using MyXml.PrProj;

namespace MyXml.PrProjXmlCommands
{
    public class EffectRequest
    {
        private readonly List<VideoFilterComponent> _videoFilters;
        private readonly List<VideoComponentChain> _videoComponentChains;

        public EffectRequest(PremiereProject project)
        {
            _videoFilters = project.GetAll<VideoFilterComponent>();
            _videoComponentChains = project.GetAll<VideoComponentChain>();
        }

        public List<string> GetAllEffectsNames()
        {
            return _videoFilters.Select(f => f.Name).Distinct().ToList();
        }

        public void ChangeEnabledEffectState(string effectName, bool enabled, int? sequenceId)
        {
            foreach (var vcc in _videoComponentChains.Where(vc => !sequenceId.HasValue || vc.SeqId == sequenceId.Value))
                vcc.SetEnabledState(effectName, enabled);
        }

        public void SetEffectTop(string effectName, int? sequenceId)
        {
            foreach (var vcc in _videoComponentChains.Where(vc => !sequenceId.HasValue || vc.SeqId == sequenceId.Value))
                vcc.SetEffectTop(effectName);
        }

        public void RemoveEffects(string effectName, int? sequenceId)
        {
            foreach (var vcc in _videoComponentChains.Where(vc => !sequenceId.HasValue || vc.SeqId == sequenceId.Value))
                vcc.RemoveEffect(effectName);
        }
    }
}
