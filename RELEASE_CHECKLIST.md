# Release Checklist

## Pre-Release Testing
- [ ] Run all automated tests
- [ ] Test on multiple Android devices
- [ ] Test all in-app purchases
- [ ] Verify ads implementation
- [ ] Test cloud saves
- [ ] Check performance metrics
- [ ] Verify privacy policy implementation
- [ ] Test offline functionality

## Store Listing
- [ ] App icon (512x512)
- [ ] Feature graphic (1024x500)
- [ ] Screenshots (min 4)
- [ ] App description
- [ ] Privacy policy URL
- [ ] Content rating questionnaire
- [ ] Contact information

## Technical Requirements
- [ ] Minimum Android version: 7.0 (API 24)
- [ ] Target Android version: 13 (API 33)
- [ ] App Bundle signed with release key
- [ ] ProGuard/R8 configuration
- [ ] Crash reporting setup
- [ ] Analytics implementation

## Monetization
- [ ] In-app products configured
- [ ] Store listing prices set
- [ ] Ad placements tested
- [ ] Revenue tracking verified

## Launch Steps
1. Build release AAB:
   ```
   Unity Editor -> Build/Android/Release
   ```

2. Testing tracks:
   - [ ] Internal testing
   - [ ] Closed testing (beta)
   - [ ] Open testing
   - [ ] Production

3. Google Play Console:
   - [ ] Upload AAB
   - [ ] Set up store listing
   - [ ] Configure in-app products
   - [ ] Set up pricing & distribution
   - [ ] Submit for review

## Post-Launch
- [ ] Monitor crash reports
- [ ] Track user analytics
- [ ] Monitor performance metrics
- [ ] Check revenue reports
- [ ] Gather user feedback
- [ ] Plan updates
