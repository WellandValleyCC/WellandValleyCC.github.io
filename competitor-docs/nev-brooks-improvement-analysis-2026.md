# Analysis of Nev Brooks Handicap Trends and Course\-Specific Benchmarks

## Table of Contents

- [Introduction](#introduction)

- [Defining Rider Improvement](#defining-rider-improvement)
  - [Generated Handicap Values](#generated-handicap-values)
  - [Maximum Improvement Ranking](#maximum-improvement-ranking)
  - [Limitations of the Measure](#limitations-of-the-measure)

- [Measuring Improvement During the 2026 Season](#measuring-improvement-during-the-2026-season)
  - [Improvement Using a Fixed 995 Benchmark](#improvement-using-a-fixed-995-benchmark)
  - [Improvement Using Course-Specific Benchmarks](#improvement-using-course-specific-benchmarks)
  - [Comparing Improvement Rankings](#comparing-improvement-rankings)

- [Comparing Benchmark Methodologies](#comparing-benchmark-methodologies)
  - [Why Course-Specific Benchmarks Were Introduced](#why-course-specific-benchmarks-were-introduced)
  - [Effect on Generated Handicap Values](#effect-on-generated-handicap-values)
  - [Effect on Improvement Rankings](#effect-on-improvement-rankings)
  - [Riders Most Affected by the Change](#riders-most-affected-by-the-change)

- [Impact on Nev Brooks Competition Results](#impact-on-nev-brooks-competition-results)
  - [Changes to Competition Standings](#changes-to-competition-standings)
  - [Largest Ranking Gains](#largest-ranking-gains)
  - [Largest Ranking Losses](#largest-ranking-losses)
  - [Competition Impact Versus Improvement Impact](#competition-impact-versus-improvement-impact)

- [Case Studies](#case-studies)
  - [Hans van Nierop: Handicap Correction Rather Than Improvement](#hans-van-nierop-handicap-correction-rather-than-improvement)
  - [Charlie Hardwicke: Course Benchmark Beneficiary](#charlie-hardwicke-course-benchmark-beneficiary)
  - [Milly Pinnock: Significant Ranking Change](#milly-pinnock-significant-ranking-change)
  - [Ian Aldridge: Improvement Revealed by Course Benchmarks](#ian-aldridge-improvement-revealed-by-course-benchmarks)

- [Findings and Conclusions](#findings-and-conclusions)
  - [Performance Versus Improvement](#performance-versus-improvement)
  - [Impact of Course-Specific Benchmarks](#impact-of-course-specific-benchmarks)
  - [Implications for the Nev Brooks Competition](#implications-for-the-nev-brooks-competition)
  - [Further Work](#further-work)

- [Appendix](#appendix)
  - [Spreadsheet Methodology](#spreadsheet-methodology)
  - [Excel Formulae](#excel-formulae)
  - [Source Data](#source-data)
  - [Treatment of Shortened Events](#treatment-of-shortened-events)
  - [Improvement Analysis](#improvement-analysis)


## Introduction

This document investigates the behaviour of the Nev Brooks handicap system during the 2026 season and evaluates the impact of introducing course\-specific benchmark times in place of the previous fixed benchmark value of 995 seconds.

The original motivation for the analysis was to determine whether the generated handicap values produced by the Nev Brooks system could be used to identify riders who genuinely improved during the season. Intuitively, a rider who is becoming stronger should, over time, require less handicap assistance to compete on equal terms with other riders. This should appear as a downward trend in the rider's generated handicap values as the season progresses.

To explore this idea, generated handicap values were extracted for each rider and analysed across the season. Several measures of improvement were considered, including trend analysis using linear regression and comparisons between a rider's earliest and best handicap values. These analyses made it possible to rank riders according to observed improvement and to compare those rankings with the final Nev Brooks competition standings.

During the investigation it became apparent that the chosen benchmark methodology had a significant influence on the generated handicaps. Two versions of the data were therefore analysed:

1. The original handicap calculations based on a fixed benchmark of 995 seconds for all courses.  This is the system used in 2026.
2. A revised version using course\-specific benchmark times derived from observed rider performances on individual courses.

The purpose of the comparison was to determine whether course\-specific benchmarks produced more realistic rider trajectories and whether they materially altered either the Nev Brooks competition standings or the interpretation of rider improvement over the season.

The analysis ultimately examines two distinct questions. First, which riders performed most successfully within the Nev Brooks competition? Second, which riders demonstrated the greatest improvement in capability during the season? Although these concepts are related, they are not necessarily the same. A rider may perform consistently well throughout the year without showing a strong improvement trend, while another rider may improve substantially yet remain lower in the final competition standings. Understanding the distinction between performance and improvement is therefore central to interpreting the results presented in this document.


## Defining Rider Improvement

The purpose of this analysis was to identify riders whose demonstrated capability improved during the 2026 season. While Nev Brooks points reward performance within the handicap competition, they do not necessarily indicate which riders improved most over the course of a season. A separate measure was therefore required.

### Generated Handicap Values

The generated handicap represents the difference between a rider's actual ride time and the benchmark time for the course ridden. Lower values indicate stronger performances relative to the benchmark. Negative values indicate performances faster than the benchmark time, while larger positive values indicate performances slower than the benchmark.

Generated handicap values provide a useful way of comparing rider performance across multiple events, particularly when different course-specific benchmarks are used.

### Maximum Improvement Ranking

A Maximum Improvement Ranking was calculated for each rider using generated handicap values recorded during the season.

For each rider, the first available generated handicap was treated as their starting point and the best (lowest) generated handicap achieved during the season was treated as their peak performance. Improvement was then measured as the reduction between those two values. Riders were ranked according to the size of this reduction, with larger reductions indicating greater apparent improvement.

This measure should not be interpreted as a measure of sustained improvement throughout the season. Instead, it captures the maximum improvement achieved relative to the rider's starting position. A rider who improved rapidly early in the season and then maintained that higher level would score highly. Equally, a rider whose performances improved gradually over many events could achieve a similar ranking if their best generated handicap occurred later in the year.

The metric should therefore be viewed as identifying riders whose demonstrated capability increased most during the season, rather than identifying riders whose performance followed the strongest continuous upward trend. In some cases, particularly where the opening handicap-setting ride was unusually slow owing to mechanical issues, poor conditions or other exceptional circumstances, the metric may partly reflect the correction of an initially inaccurate handicap estimate rather than genuine physical improvement.

### Limitations of the Measure

As a result, Maximum Improvement Rankings are best considered alongside other measures such as ride count, trend analysis and knowledge of individual rider circumstances. They provide a useful indication of how much stronger a rider became relative to their starting point, but they do not on their own explain how or when that improvement occurred.


## Measuring Improvement During the 2026 Season

To investigate rider development during the 2026 season, Maximum Improvement Rankings were calculated using generated handicap values produced by two different benchmarking approaches. The first analysis used the historical fixed benchmark value of 995 seconds for all courses. The second used course-specific benchmark values derived from observed performance data for each course.

The objective was not to determine which method produced the "correct" ranking, but rather to understand how benchmark selection influences the interpretation of rider improvement. By applying the same methodology to both datasets, it was possible to compare the resulting rankings and identify riders whose apparent improvement was particularly sensitive to the benchmark model used.

### Improvement Using a Fixed 995 Benchmark

The first analysis used generated handicap values calculated with a fixed benchmark of 995 seconds for all qualifying courses. This benchmark had historically been used within the Nev Brooks handicap competition and therefore provided a useful baseline for comparison.

Maximum Improvement Rankings were then calculated by comparing each rider's first generated handicap of the season with their best generated handicap achieved during the season. Riders were ranked according to the reduction achieved between those two values.

This approach implicitly assumes that all qualifying courses are of broadly similar difficulty and that a single benchmark can reasonably represent expected performance across the entire season.

### Improvement Using Course-Specific Benchmarks

The second analysis repeated the process using generated handicap values recalculated from course-specific benchmark times. These benchmark values were derived from historical ride data and were intended to better reflect differences in course difficulty.

As with the fixed benchmark analysis, each rider's first generated handicap was compared with their best generated handicap of the season, and Maximum Improvement Rankings were produced from the resulting improvements.

Because the benchmark time varied according to the course ridden, this approach sought to isolate changes in rider performance from variations caused by the characteristics of individual courses.

### Comparing Improvement Rankings

Once both sets of rankings had been calculated, the results were compared on a rider-by-rider basis. Particular attention was paid to riders whose rankings changed substantially between the two methods, as these cases provide insight into the impact of benchmark selection on the interpretation of rider development.

The comparison allows two related questions to be examined. First, does the use of course-specific benchmarks materially alter the assessment of rider improvement? Second, do the resulting rankings appear to better reflect known rider circumstances and observed performance throughout the season?

The sections that follow explore these questions in more detail and identify the riders whose improvement assessments were most affected by the introduction of course-specific benchmarks.

## Comparing Benchmark Methodologies

The introduction of course-specific benchmark times represented a significant change to the way generated handicaps were calculated. To understand the consequences of this change, maximum improvement rankings were calculated using both the historical fixed-benchmark approach and the revised course-specific benchmark approach. The resulting rankings were then compared to identify riders whose assessed improvement changed materially under the new methodology.

### Why Course-Specific Benchmarks Were Introduced

The original Nev Brooks analysis used a fixed benchmark value of 995 seconds for all qualifying events. While simple to implement and understand, this approach implicitly assumed that all courses presented a similar level of difficulty.

In practice, course characteristics vary considerably. Differences in elevation, road surface, exposure to wind, junctions and course layout all influence achievable riding times. A single benchmark therefore risks attributing course-related variation to rider performance.

Course-specific benchmarks were introduced to address this limitation. By assigning a benchmark that better reflects the expected performance on each course, the generated handicap calculation can more accurately isolate rider performance from course effects.

### Effect on Generated Handicap Values

The introduction of course-specific benchmarks altered generated handicap values for many riders. In some cases the changes were relatively small, while in others they materially changed the apparent progression of a rider through the season.

The analysis suggests that a fixed benchmark can introduce artificial variation into generated handicap values when riders participate in events across courses of differing difficulty. Course-specific benchmarks reduce this effect by aligning the benchmark more closely with the characteristics of the course being ridden.

As a consequence, some riders appeared to improve less rapidly under the revised methodology, while others appeared to improve more rapidly. Rather than representing a change in performance, these differences reflect changes in how performance is measured and normalised across events.

### Effect on Improvement Rankings

When maximum improvement rankings were recalculated using course-specific benchmarks, a number of riders experienced substantial changes in position. Some riders moved significantly higher in the rankings, while others moved lower.

These movements demonstrate that the choice of benchmark methodology can materially influence how rider development is interpreted. In several cases, riders who appeared to have improved markedly under the fixed-benchmark model showed more modest improvement under the course-specific model. Conversely, some riders whose improvement was less apparent under the original model moved substantially higher when course differences were taken into account.

The results suggest that benchmark selection has the potential to influence not only generated handicap values but also the broader narrative of rider development throughout a season.

### Riders Most Affected by the Change

Comparing rankings produced under the two methodologies highlights a small number of riders whose assessed improvement changed particularly significantly. These cases provide useful insight into the practical impact of the benchmark revision.

Some riders experienced only minor ranking movements, suggesting that their improvement assessment was relatively insensitive to the benchmark methodology used. Others moved by many places, indicating that course effects had previously played a substantial role in their generated handicap values.

The most extreme ranking changes provide valuable case studies for understanding the strengths and limitations of each approach. By examining these riders in detail it becomes possible to determine whether the course-specific benchmark methodology produces rankings that are more consistent with known rider circumstances and observed performance during the season.

The following sections explore the wider impact of the benchmark change on the Nev Brooks competition itself and examine several individual rider examples in greater detail.


## Impact on Nev Brooks Competition Results

While the previous sections examined the effect of benchmark selection on generated handicap values and improvement rankings, the benchmark change also had a direct impact on the Nev Brooks competition itself.

To assess this impact, final competition standings were compared using results calculated under both methodologies. This makes it possible to determine whether course-specific benchmarks merely alter the interpretation of rider development or whether they also influence the allocation of Nev Brooks points and final competition positions.

### Changes to Competition Standings

The comparison revealed that many riders experienced relatively small movements in the final standings. However, a number of riders moved by several places, demonstrating that the choice of benchmark methodology can materially influence competition outcomes.

These ranking changes arise because generated handicaps are used directly within the Nev Brooks scoring process. A change in generated handicap affects adjusted ride times, which in turn affects event placings and the allocation of competition points. Small changes accumulated over multiple events can therefore result in significant movements in the final standings.

The analysis demonstrates that benchmark selection is not simply a matter of statistical presentation. It has the potential to alter competitive outcomes and therefore deserves careful consideration.

### Largest Ranking Gains

Several riders improved their final Nev Brooks position substantially when course-specific benchmarks were introduced. These riders typically benefited from a more accurate representation of course difficulty, which resulted in stronger adjusted performances relative to competitors.

The most significant ranking gains provide useful examples of situations in which the fixed 995 benchmark may have understated rider performance. In these cases, accounting for variation in course difficulty appears to have produced a more favourable assessment of the rider's season.

These gains are particularly noteworthy because they represent changes to competition outcomes rather than merely changes to analytical measures such as improvement rankings.

### Largest Ranking Losses

Other riders experienced significant reductions in their final Nev Brooks positions under the course-specific benchmark model.

These cases suggest that the fixed 995 benchmark may previously have overstated performance on particular courses or under particular circumstances. Once course-specific benchmark values were applied, the resulting adjusted performances became less advantageous relative to the rest of the field.

The largest ranking losses are of particular interest because they help identify situations where course effects may previously have been influencing competition results more strongly than anticipated.

### Competition Impact Versus Improvement Impact

One of the most interesting findings from the analysis is that competition impact and improvement impact are not necessarily the same thing.

For some riders, the introduction of course-specific benchmarks produced large changes in competition standings while having relatively little effect on their assessed improvement. For other riders, the reverse was true. Their final Nev Brooks position changed very little, but their position in the improvement rankings changed substantially.

This distinction highlights the fact that the two analyses are measuring different things. The Nev Brooks competition rewards performance within a handicap-adjusted competitive framework. The improvement rankings seek to identify changes in rider capability over time. Although related, these concepts are not interchangeable.

The comparison therefore provides two complementary perspectives on the season. One describes how riders performed within the competition, while the other seeks to understand how riders developed over the course of the year. The introduction of course-specific benchmarks influences both perspectives, but not always in the same way.


## Case Studies

The following examples illustrate several of the key themes that emerged during the analysis. Together, they demonstrate the different ways in which benchmark selection can influence generated handicap values, improvement rankings and Nev Brooks competition standings.

### Hans van Nierop: Handicap Correction Rather Than Improvement

Hans van Nierop provides an example of how an unusual early-season performance can influence subsequent handicap calculations.

Hans experienced a mechanical issue during his initial handicap-setting ride. As a result, the generated handicap for that event was significantly higher than would normally be expected for a rider of his ability. The elevated handicap then provided an advantage during the next scored event before subsequent performances gradually corrected the estimate.

At first inspection this pattern appears similar to rapid improvement. However, closer examination suggests that the change primarily reflects correction of an initially inaccurate handicap rather than a genuine increase in rider capability. Hans therefore represents an important reminder that changes in generated handicap values may sometimes reflect the quality of the initial estimate rather than rider development.

This example highlights one of the limitations of any improvement measure based solely on handicap values. Context remains important when interpreting the results.

### Charlie Hardwicke: Course Benchmark Beneficiary

Charlie Hardwicke provides a clear example of a rider whose Nev Brooks competition position was strongly influenced by the introduction of course-specific benchmarks.

When results were recalculated using course-specific benchmark values, Charlie's final competition standing improved substantially. However, the effect on his improvement ranking was much more modest.

This distinction is important. The revised benchmark methodology did not fundamentally change the narrative of Charlie's season from an improvement perspective. Instead, it changed the way his performances were assessed relative to the difficulty of the courses ridden.

Charlie therefore illustrates how benchmark selection can materially affect competition outcomes even when the underlying assessment of rider development changes only slightly.

### Milly Pinnock: Significant Ranking Change

Milly Pinnock experienced one of the largest changes in Nev Brooks competition standing when course-specific benchmarks were introduced.

Unlike Charlie Hardwicke, whose revised position moved upwards, Milly's final standing became significantly less favourable under the revised methodology. This suggests that the fixed 995 benchmark was producing a different assessment of performance than the course-specific approach.

What makes this case particularly interesting is that the change occurred within the competition standings rather than within the improvement rankings. The underlying narrative of rider development changed relatively little compared with the substantial movement in competitive position.

Milly's results demonstrate that benchmark methodology can have a meaningful impact on competition outcomes and reinforce the importance of selecting benchmark values that accurately reflect course difficulty.

### Ian Aldridge: Improvement Revealed by Course-Specific Benchmarks

Ian Aldridge provides perhaps the clearest example of how benchmark selection can affect the interpretation of rider improvement.

Under the fixed 995 benchmark methodology, Ian appeared to show relatively modest improvement across the season. However, when course-specific benchmark values were used, the analysis indicated a much stronger improvement trajectory.

Interestingly, the effect on Ian's final Nev Brooks competition position was comparatively small. The major change occurred in the assessment of rider development rather than competitive performance.

This example illustrates one of the principal motivations for introducing course-specific benchmarks. By reducing the influence of course-related variation, the revised methodology appears to provide a clearer view of changes in rider capability over time.

Ian's results therefore demonstrate that a benchmark revision can reveal patterns of improvement that were previously obscured by the assumptions embedded in the original handicap model.

## Findings and Conclusions

The objective of this analysis was to investigate rider improvement during the 2026 season and to assess the effect of replacing the historical 995-second benchmark with course-specific benchmark values.

The analysis demonstrated that benchmark selection influences both the interpretation of rider development and the outcome of the Nev Brooks competition itself. While many riders were affected only modestly by the change, others experienced substantial changes in either their assessed improvement, their competition standing, or both.

### Performance Versus Improvement

One of the clearest findings from the analysis is that competition success and rider improvement are not the same thing.

The Nev Brooks competition rewards riders who consistently perform well relative to their handicap-adjusted peers. By contrast, the Maximum Improvement Ranking seeks to identify riders whose demonstrated capability increased most during the season.

Several riders who ranked highly in the Nev Brooks competition displayed only modest improvement according to the Maximum Improvement Ranking. Equally, some riders who showed strong improvement did not achieve similarly high competition placings.

This distinction helps explain why a rider can be one of the season's most successful competitors without necessarily being one of its most improved riders, and vice versa.

### Impact of Course-Specific Benchmarks

The introduction of course-specific benchmark values had a measurable impact on both generated handicap values and the resulting analysis.

For some riders, the revised benchmarks reduced apparent improvement by removing variation previously attributable to differences in course difficulty. For others, the revised benchmarks revealed improvement that had been obscured by the use of a single benchmark across multiple courses.

The analysis suggests that course-specific benchmarks provide a more nuanced representation of rider performance by separating rider effects from course effects more effectively than a single fixed benchmark value.

### Implications for the Nev Brooks Competition

The revised benchmark methodology did not simply alter analytical measures. It also changed the allocation of Nev Brooks points and therefore influenced final competition standings.

This outcome demonstrates that benchmark selection is a fundamental component of the handicap system rather than a purely technical detail. The benchmark methodology directly affects adjusted performances and therefore the competitive outcomes derived from them.

The analysis also highlights an inherent characteristic of handicap competitions. As riders improve, the handicap system progressively adjusts to neutralise that advantage. Consequently, a rider's improvement does not automatically translate into competition success. Improvement and competitive achievement therefore remain related but distinct concepts.

### Further Work

Several opportunities for further analysis were identified during the course of this investigation.

The Maximum Improvement Ranking captures the largest observed improvement achieved by a rider relative to their starting point. Alternative measures could be explored, including season-long trend analysis, regression-based improvement metrics or rolling handicap averages.

It may also be valuable to investigate the stability of generated handicaps over multiple seasons and to compare the behaviour of different benchmarking approaches using larger datasets.

Finally, the relationship between rider improvement and Nev Brooks points scoring could be explored in greater depth to determine whether complementary awards or metrics might better recognise rider development alongside competitive success.



## Appendix

### Spreadsheet Methodology

The analysis was performed using exported Nev Brooks results containing rider details, event information, generated handicap values and competition standings.

Separate datasets were prepared using both the historical 995 benchmark methodology and the revised course-specific benchmark methodology. Equivalent calculations were then performed on each dataset to facilitate comparison.

Maximum Improvement Rankings were created by comparing each rider's first generated handicap of the season with their best generated handicap achieved during the season.

Additional comparison sheets were created to identify differences in improvement rankings and competition standings between the two benchmark methodologies.

### Excel Formulae

The analysis relied primarily on standard Excel functions, including:

- `UNIQUE()` and `SORT()` to generate rider lists.
- `XLOOKUP()` to retrieve values from comparison sheets.
- `COUNTIFS()` to count qualifying rides.
- `SLOPE()` to calculate trend gradients.
- `RSQ()` to calculate coefficients of determination.
- `RANK()` and `RANK.EQ()` to generate ranking positions.
- `ABS()` to measure the magnitude of ranking changes.

Additional derived columns were used to calculate ranking differences and compare rider positions under the two benchmark methodologies.

### Source Data

The source data consisted of generated Nev Brooks results for the 2026 season.

Two versions of the data were analysed:

- Results calculated using a fixed benchmark value of 995 seconds.
- Results calculated using course-specific benchmark values derived from observed course performance data.

The datasets included both rider-level and event-level information sufficient to calculate generated handicaps, competition standings and season-long improvement measures.

### Treatment of Shortened Events
#### Background

A review was undertaken to determine whether shortened ten-mile events could be used to generate Nev Brooks handicaps.

The original Nev Brooks design excluded shortened events from handicap generation. As part of the 2027 review, an alternative approach was investigated.

The proposed alternative was:

Derive a benchmark time for the shortened course using the same methodology as a full-distance course.

Calculate a generated handicap relative to that benchmark.

Scale the resulting handicap to a ten-mile equivalent using the ratio:

Generated Handicap = (Ride Time - Benchmark Time) × (10 ÷ Course Miles)

#### Findings

The arithmetic was found to be correct and internally consistent.

However, comparison with surrounding full-distance events showed that handicaps generated from the shortened event were frequently inconsistent with handicaps generated from normal ten-mile events.

For example, riders often generated significantly smaller handicaps from the shortened event than from full-distance events immediately before and after it.

The investigation concluded that the concern was not the benchmark calculation itself. Rather, the concern was the assumption that performance differences scale linearly with distance.

A shortened event represents a substantially different physiological effort from a full ten-mile event. Stronger riders may be able to ride proportionally closer to their maximum effort over a shorter duration, while less experienced riders may derive less benefit from the reduced distance.

As a result, a mathematically correct scaling factor does not necessarily produce a fair ten-mile equivalent handicap.

#### Decision

Shortened events will continue to award Nev Brooks points using the rider's existing handicap.

However, shortened events will not generate a new handicap.

#### Rationale

This approach:

- Avoids making assumptions about how performance scales between o different race distances.
- Prevents shortened events disproportionately influencing a rider's established handicap.
- Maintains consistency between all handicap-generating events.
- Is straightforward to explain and administer.
- Continues to allow shortened events to participate fully within the Nev Brooks competition.

This decision may be reviewed in future if additional shortened-event data becomes available.

### Improvement Analysis
#### Background

As part of the review of the Nev Brooks competition, an investigation was undertaken into rider improvement during the 2026 season.

For each rider, a linear regression line was fitted to their ride times over the season. The slope of the regression line was then used as an indicator of improvement.

A negative slope indicates improving performance (times becoming faster over the season), while a positive slope indicates declining performance.

#### Interpretation

The improvement ranking is not part of the Nev Brooks competition and has no impact on rider scores or standings.

Its purpose is simply to identify season-long performance trends.

In general:

- Large negative slopes indicate substantial improvement.
- Slopes close to zero indicate broadly stable performance.
- Positive slopes indicate declining performance.

#### Effect of Sample Size

The analysis showed that sample size has a significant influence on confidence in the result.

For example, the two strongest regression slopes were produced by riders with only four recorded rides. While these results may represent genuine improvement, they are based on relatively little data and are therefore more sensitive to individual performances.

A strong regression slope based on four rides is less reliable than a weaker regression slope based on nine or more rides.

#### Most Improved Rider

The strongest evidence of sustained improvement during the season was provided by Ian Aldridge.

Ian recorded:

- 9 qualifying rides
- A regression slope of approximately -9 seconds per event
- An R² value of approximately 0.52

Although other riders exhibited larger negative slopes, those results were based on only four rides and are therefore less statistically reliable.

Excluding riders with fewer than five rides, Ian Aldridge emerged as the rider showing the strongest sustained improvement over the course of the season.

#### Conclusion

The improvement analysis suggests that Ian Aldridge was the most improved rider during the 2026 season.

This conclusion is based not on a single exceptional performance, but on a consistent improvement trend observed across a substantial number of rides.

The analysis also provides an interesting contrast with the Nev Brooks competition, which rewards riders who outperform their established handicap. The two measures therefore assess different aspects of rider performance and should not be expected to identify the same riders.