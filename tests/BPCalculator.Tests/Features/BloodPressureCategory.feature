Feature: Blood pressure category
  Validate that category calculation reflects systolic and diastolic values.

  Scenario Outline: Categorize a reading
    Given a systolic value of <systolic> and diastolic value of <diastolic>
    When I evaluate the blood pressure category
    Then the result should be <category>

    Examples:
      | systolic | diastolic | category |
      | 150      | 95        | High     |
      | 140      | 70        | High     |
      | 130      | 75        | PreHigh  |
      | 110      | 85        | PreHigh  |
      | 110      | 70        | Ideal    |
      | 80       | 50        | Low      |
