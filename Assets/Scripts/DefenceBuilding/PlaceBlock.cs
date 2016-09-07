﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject[] blocks;
    private int index;
    private List<GameObject> allBounds = new List<GameObject>();
    private Ray ray;
    private RaycastHit hit;

    void Start()
    {
        index = 0;
        allBounds.AddRange(GameObject.FindGameObjectsWithTag("BuildZone"));
    }

    void Update()
    {
        BlockSelect();
        PlacePoint();
    }

    void BlockSelect()
    {
        if (Input.GetAxisRaw("ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.M))
        {
            if (index == blocks.Length - 1)
            {
                index = 0;
            }

            else
            {
                index++;
            }

            Debug.Log(blocks[index]);
        }

        if (Input.GetAxisRaw("ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.N))
        {
            if (index == 0)
            {
                index = blocks.Length - 1;
            }

            else
            {
                index--;
            }

            Debug.Log(blocks[index]);
        }
    }

    void PlacePoint()
    {
        if (blocks[index] != null && Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Vector3.Distance(hit.point, transform.position) < 4.0f)
                {
                    Vector3 placePos;

                    if (hit.transform.tag == "Base")
                    {
                        placePos = new Vector3(hit.transform.position.x, hit.transform.position.y + 0.5f, hit.transform.position.z);
                    }

                    else
                    {
                        placePos = hit.transform.position + hit.normal;
                    }

                    foreach (GameObject go in allBounds)
                    {
                        if (go.GetComponent<BoxCollider>().bounds.Contains(placePos))
                        {
                            int x, y, z;
                            GameObject block = Instantiate(blocks[index], placePos, Quaternion.identity) as GameObject;

                            if ((placePos.x - 0.5f) % 2 == 1)
                            {
                                x = Mathf.RoundToInt(placePos.x - go.transform.position.x) - 1;
                            }

                            else
                            {
                                x = Mathf.RoundToInt(placePos.x - go.transform.position.x);
                            }

                            if ((placePos.y - 0.5f) % 2 == 1)
                            {
                                y = Mathf.RoundToInt(placePos.y - go.transform.position.y) - 1;
                            }

                            else
                            {
                                y = Mathf.RoundToInt(placePos.y - go.transform.position.y);
                            }

                            if ((placePos.z - 0.5f) % 2 == 1)
                            {
                                z = Mathf.RoundToInt(placePos.z - go.transform.position.z) - 1;
                            }

                            else
                            {
                                z = Mathf.RoundToInt(placePos.z - go.transform.position.z);
                            }

                            block.GetComponent<BaseBlock>().SetBZSpace(x, y, z);
                            block.GetComponent<BaseBlock>().SetBounds(go.GetComponent<BoundingBox>());
                            go.GetComponent<BoundingBox>().AddBlock(block.GetComponent<BaseBlock>());

                            if (block.tag == "VariableBlock" && hit.transform.tag == "VariableBlock")
                            {
                                block.GetComponent<Variable>().SetParent(hit.transform);
                                block.transform.rotation = block.transform.parent.rotation;
                            }
                        }
                    }
                }
            }
        }
    }
}