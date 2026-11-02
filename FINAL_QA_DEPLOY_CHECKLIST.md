# Final QA and Deployment Checklist

Motion/audio baseline is locked. Do not increase effects unless testing reveals a concrete issue.

## 1) Mobile and Browser Testing

Test matrix:
- Desktop: Chrome, Edge, Firefox, Safari (if available)
- Mobile: Android Chrome, iOS Safari

Verify on each target:
- Splash plays once and transitions cleanly to the page
- Audio starts muted by default
- Audio toggle works and state persists
- Reduced motion path works when OS setting is enabled
- No visual clipping, stutter spikes, or stuck black frame
- Main page scroll is restored after splash ends

## 2) Lighthouse and Performance Validation

Build and preview:

```powershell
npm run build
npm run preview -- --host 127.0.0.1 --port 4173
```

Run Lighthouse (non-interactive):

```powershell
npx lighthouse http://127.0.0.1:4173 --only-categories=performance --quiet --chrome-flags="--headless=new --no-sandbox" --no-enable-error-reporting --output=json --output=html --output-path=./lighthouse-performance
```

If Windows temp permission errors appear (EPERM on lighthouse temp folder):

```powershell
Remove-Item "$env:TEMP\lighthouse*" -Recurse -Force -ErrorAction SilentlyContinue
```

Then re-run the Lighthouse command.

Performance acceptance guidance:
- No severe long-task spikes during splash on target devices
- No regressions in CLS/LCP after splash handoff
- Keep large three.js chunk isolated behind lazy splash load

## 3) Deployment Verification

Pre-deploy checks:
- Production build succeeds (`npm run build`)
- Preview starts successfully (`npm run preview`)
- Network tab confirms hashed static asset loading from dist output
- No runtime errors in console during splash or handoff

Post-deploy checks:
- Open deployed URL on desktop and mobile
- Confirm splash behavior matches local preview
- Confirm caching headers are correct for hashed assets
- Re-run Lighthouse on deployed URL

## 4) Small Cleanup

Suggested minimal cleanup only:
- Keep this cinematic baseline unchanged
- Remove temporary local test artifacts if created
- Record final Lighthouse JSON/HTML files for release notes
